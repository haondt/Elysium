using Elysium.GrainInterfaces.Client;
using Microsoft.Extensions.Logging;
using Orleans.Utilities;

namespace Elysium.Grains.Client
{
    public class ClientActorActivityDeliveryGrain(ILogger<ClientActorActivityDeliveryGrain> logger) : Grain, IClientActorActivityDeliveryGrain
    {
        // todo: after 5 min of inactivity the websocket stays alive but this subsmanager stops delivering to the observer and the websock doesn't
        // receive any more updates. We should implement a heartbeat or something to keep this alive
        // we shouldn't just hold the observer forever, since the websocket may have errors and fail to call the unsubscribe
        private readonly ObserverManager<IClientActorActivityDeliveryObserver> _subsManager = new(
            TimeSpan.FromMinutes(5), logger); // todo: appsettings that shit

        public Task Subscribe(IClientActorActivityDeliveryObserver observer)
        {
            _subsManager.Subscribe(observer, observer);
            return Task.CompletedTask;
        }
        public Task Unsubscribe(IClientActorActivityDeliveryObserver observer)
        {
            _subsManager.Unsubscribe(observer);
            return Task.CompletedTask;
        }

        public Task PublishAsync(ClientIncomingActivityDetails details)
        {
            return _subsManager.Notify(s => s.ReceiveActivity(details));
        }
    }
}
