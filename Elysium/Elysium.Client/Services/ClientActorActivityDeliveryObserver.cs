using Elysium.Client.Hubs;
using Elysium.GrainInterfaces.Client;
using Elysium.Hosting.Services;
using Haondt.Web.Core.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Client.Services
{
    public class ClientActorActivityDeliveryObserver(
        HttpContext httpContext,
        string connectionId,
        IHubContext<ElysiumHub> hub, 
        IIriService iriService,
        IComponentFactory componentFactory) : IClientActorActivityDeliveryObserver
    {
        private int _counter = 0;
        public async Task ReceiveActivity(ClientIncomingActivityDetails details)
        {
            //await hub.Clients.User(localizedUsername).SendAsync("Receive", details.Type);
            await hub.Clients.Client(connectionId).SendAsync("ReceiveMessage", $"Hello is anyone there!! {_counter}");
            _counter++;
        }
    }
}
