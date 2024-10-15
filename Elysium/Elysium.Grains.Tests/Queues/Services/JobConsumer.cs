using Elysium.Grains.Queueing;
using Elysium.Grains.Tests.Queues.Grains;

namespace Elysium.Grains.Tests.Queues.Services
{
    public class JobConsumer(IGrainFactory grainFactory) : IQueueConsumer<Job>
    {
        public Task ConsumeAsync(Job payload)
        {
            return grainFactory.GetGrain<IJobConsumerGrain>(Guid.Empty).OnConsumeAsync(payload);
        }
    }
}
