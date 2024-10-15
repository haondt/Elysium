using Elysium.ActivityPub.Helpers;
using Elysium.ActivityPub.Models;
using Elysium.Core.Models;
using Elysium.Domain.Services;
using Elysium.GrainInterfaces.Client;
using Elysium.GrainInterfaces.InstanceActor;
using Elysium.GrainInterfaces.LocalActor;
using Elysium.GrainInterfaces.Services.GrainFactories;
using Elysium.Grains.Queueing;
using Haondt.Core.Models;

namespace Elysium.Grains.LocalActor
{
    public class LocalActorIncomingProcessingQueueConsumer(
        IGrainFactory<LocalIri> localIriGrainFactory,
        IGrainFactory grainFactory,
        IDocumentService documentService,
        IJsonLdService jsonLdService) : IQueueConsumer<LocalActorIncomingProcessingData>
    {
        private readonly IInstanceActorAuthorGrain _instanceAuthorGrain = grainFactory.GetGrain<IInstanceActorAuthorGrain>(Guid.Empty);

        public async Task ConsumeAsync(LocalActorIncomingProcessingData payload)
        {
            var actorAuthorGrain = localIriGrainFactory.GetGrain<ILocalActorAuthorGrain>(payload.ActorIri);
            var _clientDeliveryGrain = localIriGrainFactory.GetGrain<IClientActorActivityDeliveryGrain>(payload.ActorIri);
            var expanded = await jsonLdService.ExpandAsync(actorAuthorGrain, payload.Activity);
            var type = ActivityPubJsonNavigator.GetType(expanded);
            var typeEnumValue = ActivityType.Unknown;
            if (Enum.TryParse<ActivityType>(type, out var parsedType))
                typeEnumValue = parsedType;

            var profile = await documentService.GetExpandedDocumentAsync(_instanceAuthorGrain, payload.Sender);
            Optional<string> preferredUsername = profile.IsSuccessful
                ? ActivityPubJsonNavigator.TryGetPreferredUsername(profile.Value)
                : new();


            // todo: should probably batch these
            await _clientDeliveryGrain.PublishAsync(new ClientIncomingActivityDetails
            {
                Sender = payload.Sender,
                ExpandedObject = ActivityPubJsonNavigator.GetObject(expanded),
                SenderPreferredUsername = preferredUsername,
                Receiver = payload.ActorIri,
                Type = typeEnumValue
            });
        }
    }
}
