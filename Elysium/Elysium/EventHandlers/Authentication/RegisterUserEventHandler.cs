using Elysium.Components.Components;
using Elysium.Extensions;
using Elysium.Hosting.Services;
using Haondt.Web.Core.Components;
using Haondt.Web.Core.Http;
using Microsoft.Extensions.Options;

namespace Elysium.EventHandlers.Authentication
{
    public class RegisterUserEventHandler(
        IOptions<RegistrationSettings> registrationOptions,
        IHostingService hostingService,
        IComponentFactory componentFactory,
        InviteStateAgnosticRegisterUserEventHandler agnosticHandler) : ISingleEventHandler
    {
        public string EventName => "RegisterUser";

        public virtual async Task<IComponent> HandleAsync(IRequestData requestData)
        {
            if (registrationOptions.Value.InviteOnly)
                return await componentFactory.GetPlainComponentWithStatusCode(new RegisterModalModel
                {
                    Host = hostingService.Host,
                    Errors = ["This server is invite-only."],
                }, 403);

            var (_, result) = await agnosticHandler.HandleAsync(requestData);
            if (!result.IsSuccessful)
                result.Reason.Throw();
            return result.Value;
        }
    }
}
