namespace Elysium.GrainInterfaces.RemoteActor
{
    public interface IDispatchRemoteActivityGrain : IGrainWithGuidKey
    {
        Task Send(DispatchRemoteActivityData data);
        Task NotifyOfWorkerCompletion(long workerId);
    }
}
