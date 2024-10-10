using Elysium.ActivityPub.Helpers.ActivityCompositor;
using Elysium.ActivityPub.Models;
using Elysium.Authentication.Services;
using Elysium.Client.Services;
using Elysium.Components.Components;
using Haondt.Web.Core.Components;
using Haondt.Web.Core.Extensions;
using Haondt.Web.Core.Http;

namespace Elysium.EventHandlers.PublishActivity
{
    public class SendMessageEventHandler(
        ISessionService sessionService,
        IElysiumService elysiumService,
        IActivityPubClientService activityPubService,
        IComponentFactory componentFactory) : ISingleEventHandler
    {
        public string EventName => "SendMessage";
        public async Task<IComponent> HandleAsync(IRequestData requestData)
        {

            if (!sessionService.IsAuthenticated())
                return await componentFactory.GetPlainComponent<LoginModel>(configureResponse: m => m.SetStatusCode = 401);

            var userKey = await sessionService.GetUserKeyAsync();
            if (!userKey.HasValue)
                return await componentFactory.GetPlainComponent<LoginModel>(configureResponse: m => m.SetStatusCode = 401);

            var messageResult = requestData.Form.TryGetValue<string>("message");
            if (!messageResult.HasValue)
                return await componentFactory.GetPlainComponent(new TemporaryMessageComponentUpdateModel
                {
                    ErrorMessage = "message cannot be empty"
                });
            var recepientResult = requestData.Form.TryGetValue<string>("recepient");
            if (!recepientResult.HasValue)
                return await componentFactory.GetPlainComponent(new TemporaryMessageComponentUpdateModel
                {
                    ErrorMessage = "recipient cannot be empty"
                });
            var recepientIri = await elysiumService.GetIriForFediverseUsernameAsync(recepientResult.Value);
            if (!recepientIri.IsSuccessful)
                return await componentFactory.GetPlainComponent(new TemporaryMessageComponentUpdateModel
                {
                    ErrorMessage = recepientIri.Reason
                });

            var activityObjectDetails = new MessageDetails
            {
                Ownership = new OwnershipCompositionDetail
                {
                    AttributedTo = new(await activityPubService.GetLocalIriFromUserIdentityAsync(userKey.Value))
                },
                Text = messageResult.Value,
                Addressing = new AddressingCompositionDetail
                {
                    To = [recepientIri.Value]
                },
            };

            var activityObject = activityObjectDetails.Composit();

            try
            {
                var (activityIri, activity) = await activityPubService.PublishActivityAsync(userKey.Value, ActivityType.Create, activityObject);
            }
            catch
            {
                return await componentFactory.GetPlainComponent(new TemporaryMessageComponentUpdateModel
                {
                    ErrorMessage = "failed to send activity"
                });
            }

            return await componentFactory.GetPlainComponent(new TemporaryMessageComponentUpdateModel
            {
                NotifySuccess = true,
            });
        }
    }
}
