
using Haondt.Identity.StorageKey;

namespace Elysium.GrainInterfaces.Queueing
{
    public interface IQueueWorkerGrain<T> : IGrain<QueueWorkerIdentity>
    {
        Task WorkAsync(StorageKey<T> key, T payload);
    }
}
