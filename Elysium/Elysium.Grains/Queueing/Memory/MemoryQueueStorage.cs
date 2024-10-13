using Haondt.Identity.StorageKey;
using System.Collections.Concurrent;
using System.Threading.Channels;

namespace Elysium.Grains.Queueing.Memory
{
    public class MemoryQueueStorage<T>(
        Channel<(StorageKey<T> Key, T Payload)> queue,
        ConcurrentDictionary<StorageKey<T>, T> stage) : IQueueStorage<T>
    {

        public async Task<(StorageKey<T> Key, T Payload)> BlockingDequeueToStage()
        {
            var next = await queue.Reader.ReadAsync();
            if (!stage.TryAdd(next.Key, next.Payload))
                throw new InvalidOperationException($"key {next.Key} already staged");
            return (next.Key, next.Payload);
        }

        public Task CommitDequeue(StorageKey<T> key)
        {
            stage.Remove(key, out _);
            return Task.CompletedTask;
        }

        public async Task<StorageKey<T>> Enqueue(T payload)
        {
            var key = StorageKey<MemoryQueueStorage>.Create(Guid.NewGuid().ToString()).Extend<T>();
            await queue.Writer.WriteAsync((key, payload));
            return key;
        }
    }

    public class MemoryQueueStorage { }
}