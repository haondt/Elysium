using Haondt.Identity.StorageKey;
using Orleans.Concurrency;

namespace Elysium.GrainInterfaces.Queueing
{
    public interface IQueueGrain : IGrainWithStringKey
    {
        [AlwaysInterleave]
        Task EnsureActivatedAsync();
    }

    public interface IQueueGrain<T> : IQueueGrain
    {
        [AlwaysInterleave]
        Task NotifyWorkCompleteAsync(int workerId, StorageKey<T> key, bool success);
    }
}
