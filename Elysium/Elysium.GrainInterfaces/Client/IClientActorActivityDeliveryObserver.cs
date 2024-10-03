namespace Elysium.GrainInterfaces.Client
{
    public interface IClientActorActivityDeliveryObserver : IGrainObserver
    {
        Task ReceiveActivity(ClientIncomingActivityDetails details);
    }
}
