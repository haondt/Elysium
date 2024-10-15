

using Elysium.Grains.Tests.Queues.Services;

namespace Elysium.Grains.Tests.Queues.Grains
{
    public interface IJobConsumerGrain : IGrainWithGuidKey
    {
        Task BlockExecutionAsync(RentedQueue rentedQueue, Guid jobId);

        //Task BlockExecution(RentedQueue rentedQueue);
        Task<List<(DateTime Timestamp, Job Payload)>> GetHistoryAsync(RentedQueue queue);
        Task OnConsumeAsync(Job payload);
        Task<RentedQueue> RentQueueAsync();
        Task ReturnQueueAsync(RentedQueue rentedQueue);
        Task UnblockExecutionAsync(RentedQueue rentedQueue, Guid jobId);

        //Task UnblockExecution(RentedQueue rentedQueue);
        ValueTask<Job> WatchQueueAsync(RentedQueue queue);
    }
}
