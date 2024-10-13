using Elysium.Grains.Queueing;

namespace Elysium.Grains.LocalActor
{
    public class LocalActorOutgoingProcessingQueueConsumer : IQueueConsumer<LocalActorOutgoingProcessingData>
    {
        public Task ConsumeAsync(LocalActorOutgoingProcessingData payload)
        {
            throw new NotImplementedException();
        }

    }
}
