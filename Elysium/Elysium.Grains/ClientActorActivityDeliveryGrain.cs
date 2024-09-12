using Elysium.GrainInterfaces;
using Elysium.GrainInterfaces.Client;
using Microsoft.Extensions.Logging;
using Orleans.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains
{
    public class ClientActorActivityDeliveryGrain(ILogger<ClientActorActivityDeliveryGrain> logger) : Grain, IClientActorActivityDeliveryGrain
    {
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
