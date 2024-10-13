namespace Elysium.GrainInterfaces.Queueing
{
    public interface IQueueStartupGrain : IGrainWithGuidKey
    {
        Task EnsureActivatedAsync();
    }
}
