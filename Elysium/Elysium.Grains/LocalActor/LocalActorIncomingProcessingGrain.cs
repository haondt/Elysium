using Elysium.ActivityPub.Helpers;
using Elysium.ActivityPub.Models;
using Elysium.Core.Models;
using Elysium.Domain.Services;
using Elysium.GrainInterfaces.Client;
using Elysium.GrainInterfaces.Constants;
using Elysium.GrainInterfaces.InstanceActor;
using Elysium.GrainInterfaces.LocalActor;
using Elysium.GrainInterfaces.Services;
using Elysium.Hosting.Services;
using Haondt.Core.Models;
using Microsoft.Extensions.Logging;
using Orleans.Streams;

namespace Elysium.Grains.LocalActor
{
    [ImplicitStreamSubscription(GrainConstants.LocalActorIncomingProcessingStream)]
    public class LocalActorIncomingProcessingGrain : Grain, ILocalActorIncomingProcessingGrain
    {
        private readonly IInstanceActorAuthorGrain _instanceAuthorGrain;
        private readonly ILocalActorAuthorGrain _actorAuthorGrain;
        private readonly LocalIri _id;
        private readonly IClientActorActivityDeliveryGrain _clientDeliveryGrain;
        private readonly IDocumentService _documentService;
        private readonly IJsonLdService _jsonLdService;
        private readonly IIriService _iriService;
        private readonly ILogger<LocalActorIncomingProcessingGrain> _logger;
        private StreamSubscriptionHandle<LocalActorIncomingProcessingData>? _subscription;

        public LocalActorIncomingProcessingGrain(IGrainFactory<LocalIri> grainFactory,
            IGrainFactory baseGrainFactory,
            IDocumentService documentService,
            IJsonLdService jsonLdService,
            IIriService iriService,
            ILogger<LocalActorIncomingProcessingGrain> logger)
        {
            _id = grainFactory.GetIdentity(this);
            _clientDeliveryGrain = grainFactory.GetGrain<IClientActorActivityDeliveryGrain>(_id);
            _instanceAuthorGrain = baseGrainFactory.GetGrain<IInstanceActorAuthorGrain>(Guid.Empty);
            _actorAuthorGrain = grainFactory.GetGrain<ILocalActorAuthorGrain>(_id);
            _documentService = documentService;
            _jsonLdService = jsonLdService;
            _iriService = iriService;
            _logger = logger;
        }
        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            var streamProvider = this.GetStreamProvider(GrainConstants.SimpleStreamProvider);
            var streamId = StreamId.Create(GrainConstants.LocalActorIncomingProcessingStream, _id.Iri.ToString());
            var stream = streamProvider.GetStream<LocalActorIncomingProcessingData>(streamId);
            _subscription = await stream.SubscribeAsync(OnNextAsync);
            await base.OnActivateAsync(cancellationToken);
        }
        public override async Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
        {
            if (_subscription != null)
                await _subscription.UnsubscribeAsync();
            await base.OnDeactivateAsync(reason, cancellationToken);
        }

        public async Task OnNextAsync(LocalActorIncomingProcessingData data, StreamSequenceToken? token)
        {
            var swallowExceptions = true;
            if (swallowExceptions)
                try
                {
                    await OnNextAsyncInternal(data, token);
                }
                catch
                {
                    // todo: log exception
                }
            else
                await OnNextAsyncInternal(data, token);

        }

        public async Task OnNextAsyncInternal(LocalActorIncomingProcessingData data, StreamSequenceToken? token)
        {
            var expanded = await _jsonLdService.ExpandAsync(_actorAuthorGrain, data.Activity);
            var type = ActivityPubJsonNavigator.GetType(expanded);
            var typeEnumValue = ActivityType.Unknown;
            if (Enum.TryParse<ActivityType>(type, out var parsedType))
                typeEnumValue = parsedType;

            var profile = await _documentService.GetExpandedDocumentAsync(_instanceAuthorGrain, data.Sender);
            Optional<string> preferredUsername = profile.IsSuccessful
                ? ActivityPubJsonNavigator.TryGetPreferredUsername(profile.Value)
                : new();


            // todo: should probably batch these
            await _clientDeliveryGrain.PublishAsync(new ClientIncomingActivityDetails
            {
                Sender = data.Sender,
                ExpandedObject = ActivityPubJsonNavigator.GetObject(expanded),
                SenderPreferredUsername = preferredUsername,
                Receiver = _id,
                Type = typeEnumValue
            });
        }
    }
}
