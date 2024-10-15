using Elysium.Grains.Tests.Queues.Services;

namespace Elysium.Grains.Tests.Queues.Grains
{
    public interface IJobProducerGrain : IGrainWithGuidKey
    {
        Task<Job> ProduceJobAsync(string queueName, string payload);
        Task<Job> ProduceJobAsync(string queueName, Guid jobId, string payload);
    }
}
