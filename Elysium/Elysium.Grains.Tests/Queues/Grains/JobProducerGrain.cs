using Elysium.Grains.Queueing;
using Elysium.Grains.Tests.Queues.Services;

namespace Elysium.Grains.Tests.Queues.Grains
{
    public class JobProducerGrain(IQueueProvider queueProvider) : Grain, IJobProducerGrain
    {
        public Task<Job> ProduceJobAsync(string queueName, string payload)
        {
            return ProduceJobAsync(queueName, Guid.NewGuid(), payload);
        }

        public async Task<Job> ProduceJobAsync(string queueName, Guid jobId, string payload)
        {
            var queue = queueProvider.GetQueue<Job>(queueName);
            var job = new Job
            {
                Payload = payload,
                SourceQueue = queueName,
                Id = jobId
            };

            await queue.EnqueueAsync(job);
            return job;
        }
    }
}
