using Elysium.Core.Models;
using Elysium.GrainInterfaces.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces
{
    public interface IClientActorActivityDeliveryGrain : IGrain<LocalIri>
    {
        Task PublishAsync(ClientIncomingActivityDetails details);
        Task Subscribe(IClientActorActivityDeliveryObserver observer);
        Task Unsubscribe(IClientActorActivityDeliveryObserver observer);
    }
}
