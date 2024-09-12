using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces.Client
{
    public interface IClientActorActivityDeliveryObserver : IGrainObserver
    {
        Task ReceiveActivity(ClientIncomingActivityDetails details);
    }
}
