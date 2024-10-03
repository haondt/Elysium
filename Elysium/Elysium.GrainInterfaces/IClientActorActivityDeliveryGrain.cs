using Elysium.Core.Models;
using Elysium.GrainInterfaces.Client;

namespace Elysium.GrainInterfaces
{
    public interface IClientActorActivityDeliveryGrain : IGrain<LocalIri>
    {
        Task PublishAsync(ClientIncomingActivityDetails details);
        Task Subscribe(IClientActorActivityDeliveryObserver observer);
        Task Unsubscribe(IClientActorActivityDeliveryObserver observer);
    }
}
