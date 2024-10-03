using Elysium.Client.Services;
using Elysium.Core.Models;
using Haondt.Core.Models;
using System.Collections.Concurrent;

namespace Elysium.Client.Hubs
{
    public class ClientActorActivityDeliveryObserverRegistry : IClientActorActivityDeliveryObserverRegistry
    {
        // necessary because the grain only holds on to observers with a weak reference
        ConcurrentDictionary<string, (LocalIri Iri, ClientActorActivityDeliveryObserver Observer)> _observers = new();
        public void RegisterObserver(string connectionId, LocalIri iri, ClientActorActivityDeliveryObserver observer)
        {
            if (_observers.ContainsKey(connectionId))
                throw new ArgumentException($"Connection ID {connectionId} already registered");

            _observers[connectionId] = (iri, observer);
        }

        public Optional<(LocalIri Iri, ClientActorActivityDeliveryObserver Observer)> UnregisterObserver(string connectionId)
        {
            if (_observers.Remove(connectionId, out var tup))
                return new(tup);
            return new();
        }
    }
}
