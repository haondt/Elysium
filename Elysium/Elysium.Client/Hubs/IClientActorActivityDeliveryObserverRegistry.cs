using Elysium.Client.Services;
using Elysium.Core.Models;
using Haondt.Core.Models;

namespace Elysium.Client.Hubs
{
    public interface IClientActorActivityDeliveryObserverRegistry
    {
        void RegisterObserver(string connectionId, LocalIri iri, ClientActorActivityDeliveryObserver observer);
        Optional<(LocalIri Iri, ClientActorActivityDeliveryObserver Observer)> UnregisterObserver(string connectionId);
    }
}
