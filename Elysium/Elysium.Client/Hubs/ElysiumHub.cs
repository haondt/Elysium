using Elysium.Authentication.Services;
using Elysium.Client.Services;
using Elysium.Core.Models;
using Elysium.GrainInterfaces;
using Elysium.GrainInterfaces.Client;
using Elysium.GrainInterfaces.Services;
using Elysium.Hosting.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Client.Hubs
{
    public class ElysiumHub(
        IIriService iriService,
        IGrainFactory grainFactory,
        IClientActorActivityDeliveryObserverRegistry registry,
        IGrainFactory<LocalIri> localGrainFactory) : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await TryLinkDeliveryGrainAsync();
            await base.OnConnectedAsync();
        }

        private async Task TryLinkDeliveryGrainAsync()
        {

            var httpContext = Context.GetHttpContext();
            if (httpContext == null)
                return;

            var sessionService = httpContext.RequestServices.GetRequiredService<ISessionService>();
            if (!sessionService.IsAuthenticated())
                return;

            var userIdentity = await sessionService.GetUserKeyAsync();
            if (!userIdentity.HasValue)
                return;

            var activityPubClientService = httpContext.RequestServices.GetRequiredService<IActivityPubClientService>();
            var localIri = await activityPubClientService.GetLocalIriFromUserIdentityAsync(userIdentity.Value);

            var deliveryGrain = localGrainFactory.GetGrain<IClientActorActivityDeliveryGrain>(localIri);
            var observer = ActivatorUtilities.CreateInstance<ClientActorActivityDeliveryObserver>(httpContext.RequestServices, httpContext, Context.ConnectionId);
            registry.RegisterObserver(Context.ConnectionId, localIri, observer);
            var reference = grainFactory.CreateObjectReference<IClientActorActivityDeliveryObserver>(observer);
            await deliveryGrain.Subscribe(reference);
        }

        private async Task TryUnlinkDeliveryGrainAsync()
        {
            var result = registry.UnregisterObserver(Context.ConnectionId);
            if(!result.HasValue)
                return;

            var deliveryGrain = localGrainFactory.GetGrain<IClientActorActivityDeliveryGrain>(result.Value.Iri);
            var reference = grainFactory.CreateObjectReference<IClientActorActivityDeliveryObserver>(result.Value.Observer);
            await deliveryGrain.Unsubscribe(reference);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await TryUnlinkDeliveryGrainAsync();
            await base.OnDisconnectedAsync(exception);
        }
    }
}
