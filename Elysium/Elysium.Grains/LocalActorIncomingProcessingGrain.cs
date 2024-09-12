using Elysium.ActivityPub.Helpers;
using Elysium.ActivityPub.Models;
using Elysium.Core.Extensions;
using Elysium.Core.Models;
using Elysium.GrainInterfaces;
using Elysium.GrainInterfaces.Client;
using Elysium.GrainInterfaces.Services;
using Elysium.Grains.Services;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains
{
    [ImplicitStreamSubscription(GrainConstants.LocalActorIncomingProcessingStream)]
    public class LocalActorIncomingProcessingGrain : Grain, ILocalActorIncomingProcessingGrain
    {
        private readonly IInstanceActorAuthorGrain _instanceAuthorGrain;
        private readonly ILocalActorAuthorGrain _actorAuthorGrain;
        private readonly LocalIri _id;
        private readonly IClientActorActivityDeliveryGrain _clientDeliveryGrain;
        private readonly IJsonLdService _jsonLdService;
        private readonly ILogger<LocalActorIncomingProcessingGrain> _logger;
        private StreamSubscriptionHandle<LocalActorIncomingProcessingData>? _subscription;

        public LocalActorIncomingProcessingGrain(IGrainFactory<LocalIri> grainFactory,
            IGrainFactory baseGrainFactory,
            IJsonLdService jsonLdService,
            ILogger<LocalActorIncomingProcessingGrain> logger)
        {
            _id = grainFactory.GetIdentity(this);
            _clientDeliveryGrain = grainFactory.GetGrain<IClientActorActivityDeliveryGrain>(_id);
            _instanceAuthorGrain = baseGrainFactory.GetGrain<IInstanceActorAuthorGrain>(Guid.Empty);
            _actorAuthorGrain = grainFactory.GetGrain<ILocalActorAuthorGrain>(_id);
            _jsonLdService = jsonLdService;
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
            // todo: should probably batch these
            await _clientDeliveryGrain.PublishAsync(new ClientIncomingActivityDetails
            {
                Sender = data.Sender,
                ExpandedObject = expanded,
                Receiver = _id,
                Type = typeEnumValue
            });
        }
    }
}
