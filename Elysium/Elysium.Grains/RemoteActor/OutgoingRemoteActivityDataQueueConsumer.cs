using Elysium.Core.Models;
using Elysium.Domain.Services;
using Elysium.GrainInterfaces.LocalActor;
using Elysium.GrainInterfaces.RemoteActor;
using Elysium.GrainInterfaces.Services.GrainFactories;
using Elysium.Grains.Queueing;

namespace Elysium.Grains.RemoteActor
{
    public class OutgoingRemoteActivityDataQueueConsumer(
        IActivityPubHttpService httpService,
        IGrainFactory<LocalIri> localIriGrainFactory) : IQueueConsumer<OutgoingRemoteActivityData>
    {
        public async Task ConsumeAsync(OutgoingRemoteActivityData payload)
        {
            var result = await httpService.PostAsync(new HttpPostData
            {
                JsonLdPayload = payload.Payload.ToString(),
                Target = payload.Receiver,
                Author = localIriGrainFactory.GetGrain<ILocalActorAuthorGrain>(payload.Sender)
            });

            if (!result.IsSuccessful)
                throw new HttpRequestException($"Failed to send outgoing remote actor activity due to reason {result.Reason}");
        }
    }
}
