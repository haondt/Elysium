using Elysium.Authentication.Services;
using Elysium.Client.Hubs;
using Elysium.Core.Models;
using Elysium.GrainInterfaces.Client;
using Haondt.Identity.StorageKey;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace Elysium.Client.Services
{
    public class ClientActorActivityDeliveryObserver(
        string connectionId,
        string clientType,
        StorageKey<UserIdentity> identity,
        IServiceScopeFactory scopeFactory,
        IHubContext<ElysiumHub> hub) : IClientActorActivityDeliveryObserver
    {
        public async Task ReceiveActivity(ClientIncomingActivityDetails details)
        {
            using var scope = scopeFactory.CreateScope();
            var sessionService = scope.ServiceProvider.GetRequiredService<ISessionService>();
            if (sessionService is ProxySessionService proxySessionService)
                proxySessionService.SessionService = new StaticSessionService(identity);

            var handlerFactory = scope.ServiceProvider.GetRequiredService<IClientActorActivityHandlerFactory>();
            var handler = handlerFactory.Create(clientType);

            var result = await handler.HandleAsync(details);
            if (!result.HasValue)
                return;

            if (!result.Value.Arg.HasValue)
            {
                await hub.Clients.Client(connectionId).SendAsync(result.Value.Method);
                return;
            }

            await hub.Clients.Client(connectionId).SendAsync(result.Value.Method, result.Value.Arg.Value);
        }

    }
}
