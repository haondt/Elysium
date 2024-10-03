namespace Elysium.GrainInterfaces
{
    public interface IDispatchRemoteActivityGrain : IGrainWithGuidKey
    {
        Task Send(DispatchRemoteActivityData data);
        Task NotifyOfWorkerCompletion(long workerId);
    }
}
