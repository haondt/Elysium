using Haondt.Identity.StorageKey;

namespace Elysium.Grains.Queueing
{
    public interface IQueueStorage<T>
    {
        Task<StorageKey<T>> Enqueue(T payload);
        Task<(StorageKey<T> Key, T Payload)> BlockingDequeueToStage();
        Task CommitDequeue(StorageKey<T> key);
        Task RequeueDeadletters();
    }
}
