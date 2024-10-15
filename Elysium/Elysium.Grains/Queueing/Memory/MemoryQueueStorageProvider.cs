using Haondt.Identity.StorageKey;
using System.Collections.Concurrent;
using System.Threading.Channels;

namespace Elysium.Grains.Queueing.Memory
{
    public class MemoryQueueStorageProvider : ITypedQueueStorageProvider
    {
        public QueueStorageType Type => QueueStorageType.Memory;
        private readonly ConcurrentDictionary<string, object> _queues = new();

        public IQueueStorage<T> GetStorage<T>(string name)
        {
            var queueData = _queues.GetOrAdd(name, new QueueData<T>());
            if (queueData is not QueueData<T> typedQueueData)
                throw new InvalidOperationException($"A queue with name {name} has already been added, but it has a type of {queueData.GetType()} in stead of the expected {typeof(QueueData<T>)}");
            return new MemoryQueueStorage<T>(typedQueueData.QueueChannel, typedQueueData.Stage);
        }
    }

    public class QueueData<T>
    {
        public Channel<(StorageKey<T> Key, T Payload)> QueueChannel { get; set; } = Channel.CreateUnbounded<(StorageKey<T>, T)>(new UnboundedChannelOptions
        {
            AllowSynchronousContinuations = false,
            SingleReader = true,
            SingleWriter = false
        });
        public ConcurrentDictionary<StorageKey<T>, T> Stage { get; set; } = new();
    }
}
