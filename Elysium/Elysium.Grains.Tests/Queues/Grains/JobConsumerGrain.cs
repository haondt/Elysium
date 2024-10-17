using Elysium.Grains.Queueing;
using Elysium.Grains.Tests.Queues.Services;
using Orleans.Concurrency;
using System.Collections.Concurrent;
using System.Threading.Channels;

namespace Elysium.Grains.Tests.Queues.Grains
{
    [Reentrant]
    [KeepAlive]
    public class JobConsumerGrain(IEnumerable<QueueDescriptor> descriptors) : Grain, IJobConsumerGrain
    {
        private Channel<string> _availableQueues { get; set; } = Channel.CreateUnbounded<string>();
        private ConcurrentDictionary<string, RentedQueueInfo> _rentedQueues { get; set; } = new();

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            foreach (var descriptor in descriptors)
                await _availableQueues.Writer.WriteAsync(descriptor.Name, cancellationToken);
        }

        public async Task<RentedQueue> RentQueueAsync()
        {
            var name = await _availableQueues.Reader.ReadAsync();
            var rentedQueue = new RentedQueue
            {
                Name = name,
                Consumer = this
            };

            _rentedQueues[name] = new RentedQueueInfo
            {
                Name = name
            };

            return rentedQueue;
        }

        public async Task OnConsumeAsync(Job payload)
        {
            if (!_rentedQueues.TryGetValue(payload.SourceQueue, out var rentedQueue))
                return;

            if (rentedQueue.ExecutionBlocks.TryGetValue(payload.Id, out var block))
                await block.Task;

            rentedQueue.History.Add((DateTime.UtcNow, payload));
            await rentedQueue.PayloadChannel.Writer.WriteAsync(payload);
        }

        public ValueTask<Job> WatchQueueAsync(RentedQueue queue)
        {
            if (!_rentedQueues.TryGetValue(queue.Name, out var rentedQueue))
                throw new KeyNotFoundException(queue.Name);
            return rentedQueue.PayloadChannel.Reader.ReadAsync();
        }

        public Task<List<(DateTime Timestamp, Job Payload)>> GetHistoryAsync(RentedQueue queue)
        {
            if (!_rentedQueues.TryGetValue(queue.Name, out var rentedQueue))
                throw new KeyNotFoundException(queue.Name);
            return Task.FromResult(rentedQueue.History);
        }


        public Task ReturnQueueAsync(RentedQueue rentedQueue)
        {
            _rentedQueues.Remove(rentedQueue.Name, out _);
            return _availableQueues.Writer.WriteAsync(rentedQueue.Name).AsTask();
        }

        public Task BlockExecutionAsync(RentedQueue rentedQueue, Guid jobId)
        {
            if (!_rentedQueues.TryGetValue(rentedQueue.Name, out var queueInfo))
                return Task.CompletedTask;

            if (queueInfo.ExecutionBlocks.TryGetValue(jobId, out var block)
                && !block.Task.IsCompleted)
                return Task.CompletedTask;

            queueInfo.ExecutionBlocks[jobId] = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

            return Task.CompletedTask;
        }

        public Task UnblockExecutionAsync(RentedQueue rentedQueue, Guid jobId)
        {
            if (!_rentedQueues.TryGetValue(rentedQueue.Name, out var queueInfo))
                return Task.CompletedTask;

            if (queueInfo.ExecutionBlocks.TryGetValue(jobId, out var block)
                && !block.Task.IsCompleted)
                block.SetResult();

            return Task.CompletedTask;
        }
    }

    [GenerateSerializer]
    public class RentedQueue : IAsyncDisposable
    {
        [Id(0)]
        public required string Name { get; init; }

        [Id(1)]
        public required IJobConsumerGrain Consumer { get; init; }

        public async ValueTask DisposeAsync()
        {
            await Consumer.ReturnQueueAsync(this);
        }
    }

    public class RentedQueueInfo
    {
        public required string Name { get; set; }
        public Dictionary<Guid, TaskCompletionSource> ExecutionBlocks { get; set; } = new();
        public List<(DateTime Timestamp, Job Payload)> History { get; set; } = [];

        public Channel<Job> PayloadChannel { get; set; } = Channel.CreateUnbounded<Job>();
    }
}
