using Elysium.Core.Models;

namespace Elysium.GrainInterfaces.Client
{
    public interface IClientActorActivityDeliveryGrain : IGrain<LocalIri>
    {
        Task PublishAsync(ClientIncomingActivityDetails details);
        Task Subscribe(IClientActorActivityDeliveryObserver observer);
        Task Unsubscribe(IClientActorActivityDeliveryObserver observer);
    }
}
