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
    public class CreatePostEventHandler(
        IActivityPubClientService activityPubService,
        IComponentFactory componentFactory,
        ISessionService sessionService) : ISingleEventHandler
    {
        public string EventName => "CreatePost";

        public async Task<IComponent> HandleAsync(IRequestData requestData)
        {

            if (!sessionService.IsAuthenticated())
                return await componentFactory.GetPlainComponent<LoginModel>(configureResponse: m => m.SetStatusCode = 401);

            var userKey = await sessionService.GetUserKeyAsync();
            if (!userKey.HasValue)
                return await componentFactory.GetPlainComponent<LoginModel>(configureResponse: m => m.SetStatusCode = 401);
            var userIri = await activityPubService.GetLocalIriFromUserIdentityAsync(userKey.Value);

            var titleResult = requestData.Form.TryGetValue<string>("title");
            var textResult = requestData.Form.TryGetValue<string>("text");
            var audienceResult = requestData.Form.TryGetValue<string>("audience");

            string? textValue = (textResult.HasValue && !(string.IsNullOrWhiteSpace(textResult.Value))) ? textResult.Value.Trim() : null;
            string? titleValue = (titleResult.HasValue && !(string.IsNullOrWhiteSpace(titleResult.Value))) ? titleResult.Value.Trim() : null;

            if (!audienceResult.HasValue)
                return await componentFactory.GetPlainComponent(
                    new CreatePostModalModel
                    {
                        ExistingText = textValue,
                        ExistingTitle = titleValue,
                        AudienceError = "Please select an audience."
                    },
                    configureResponse: m => m.SetStatusCode = 400);

            // todo: audience targeting

            if ((!titleResult.HasValue || string.IsNullOrWhiteSpace(titleResult.Value)) && (!textResult.HasValue || string.IsNullOrWhiteSpace(textResult.Value)))
                return await componentFactory.GetPlainComponent(
                    new CreatePostModalModel
                    {
                        HasEmptyContent = true,
                        ExistingAudience = audienceResult.Value
                    },
                    configureResponse: m => m.SetStatusCode = 400);

            var activityObjectDetails = new CreatePostDetails
            {
                Addressing = AddressingCompositionDetail.Public,
                Ownership = new OwnershipCompositionDetail { AttributedTo = userIri.Iri },
                Text = textValue,
                Title = titleValue,
            };

            try
            {
                await activityPubService.PublishActivityAsync(userKey.Value, ActivityType.Create, activityObjectDetails.Composit());
            }
            catch
            {
                //todo: this should show a popup in the ui
                throw;
            }

            // todo: this should refresh the feed
            return await componentFactory.GetPlainComponent<CloseModalModel>();
        }
    }
}
