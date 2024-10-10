using Elysium.Authentication.Exceptions;
using Elysium.Authentication.Services;
using Elysium.Client.Services;
using Elysium.Components.Components.Admin;
using Haondt.Web.Core.Components;
using Haondt.Web.Core.Http;

namespace Elysium.EventHandlers.Authentication
{
    public class GenerateInviteLinkEventHandler(
        ISessionService sessionService,
        IElysiumService elysiumService,
        IComponentFactory componentFactory) : ISingleEventHandler
    {
        public string EventName => "GenerateInvite";

        public async Task<IComponent> HandleAsync(IRequestData requestData)
        {
            if (!sessionService.IsAdministrator())
                throw new NeedsAuthorizationException();

            var link = await elysiumService.GenerateInviteLinkAsync();
            return await componentFactory.GetPlainComponent(new GenerateInviteModel
            {
                InviteLink = link
            });
        }
    }
}
