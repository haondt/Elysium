using Elysium.Client.Services;
using Elysium.Components.Components;
using Elysium.Hosting.Services;
using Elysium.Persistence.Services;
using Haondt.Web.Core.Components;
using Haondt.Web.Core.Extensions;
using Haondt.Web.Core.Http;

namespace Elysium.EventHandlers.Authentication
{
    public class InvitedRegisterUserEventHandler(
        IComponentFactory componentFactory,
        IElysiumStorage elysiumStorage,
        IHostingService hostingService,
        InviteStateAgnosticRegisterUserEventHandler agnosticHandler) : ISingleEventHandler
    {

        public string EventName => "InvitedRegisterUser";

        public async Task<IComponent> HandleAsync(IRequestData requestData)
        {
            var inviteIdResult = requestData.Form.TryGetValue<string>("inviteId");
            if (!inviteIdResult.HasValue)
                return await componentFactory.GetPlainComponent(new InvitedRegisterLayoutModel
                {
                    Host = hostingService.Host,
                    Errors = ["Invalid invite link"],
                    InviteId = ""
                });

            var inviteKeyToDelete = InviteLinkDetails.GetStorageKey(inviteIdResult.Value);
            var invite = await elysiumStorage.Get(inviteKeyToDelete);
            if (!invite.IsSuccessful)
                return await componentFactory.GetPlainComponent(new InvitedRegisterLayoutModel
                {
                    Host = hostingService.Host,
                    Errors = ["Invalid invite link"],
                    InviteId = ""
                });

            // todo: other validations on invite link, e.g. expiration

            var (createdAccount, result) = await agnosticHandler.HandleAsync(requestData);
            if (createdAccount)
                await elysiumStorage.Delete(inviteKeyToDelete);

            if (!result.IsSuccessful)
                result.Reason.Throw();

            return result.Value;
        }
    }
}
