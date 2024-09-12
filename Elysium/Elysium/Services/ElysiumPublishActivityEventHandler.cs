using Elysium.ActivityPub;
using Elysium.ActivityPub.Helpers.ActivityCompositor;
using Elysium.ActivityPub.Models;
using Elysium.Authentication.Services;
using Elysium.Client.Services;
using Elysium.Components.Components;
using Elysium.Core.Models;
using Elysium.Hosting.Services;
using Haondt.Core.Models;
using Haondt.Web.Core.Components;
using Haondt.Web.Core.Extensions;
using Haondt.Web.Core.Http;
using Haondt.Web.Services;

namespace Elysium.Services
{
    public class ElysiumPublishActivityEventHandler(
        IActivityPubClientService activityPubService,
        ISessionService sessionService,
        IElysiumService elysiumService,
        IComponentFactory componentFactory) : IEventHandler
    {
        public const string SEND_MESSAGE_EVENT = "SendMessage";

        public async Task<Optional<IComponent>> HandleAsync(string eventName, IRequestData requestData)
        {

            if (SEND_MESSAGE_EVENT.Equals(eventName))
            {
                if (!sessionService.IsAuthenticated())
                    return await GetLoginComponentAsOptionalAsync();

                var userKey = await sessionService.GetUserKeyAsync();
                if (!userKey.HasValue)
                    return await GetLoginComponentAsOptionalAsync();

                var messageResult = requestData.Form.TryGetValue<string>("message");
                if (!messageResult.HasValue)
                    return await GetMessageErrorComponentAsOptionalAsync("message cannot be empty");
                var recepientResult = requestData.Form.TryGetValue<string>("recepient");
                if (!recepientResult.HasValue)
                    return await GetMessageErrorComponentAsOptionalAsync("recepient cannot be empty");
                var recepientIri = await elysiumService.GetIriForFediverseUsernameAsync(recepientResult.Value);
                if (!recepientIri.IsSuccessful)
                    return await GetMessageErrorComponentAsOptionalAsync(recepientIri.Reason);

                var activityObjectDetails = new MessageDetails
                {
                    Text = messageResult.Value,
                    Recepient = recepientIri.Value,
                    AttributedTo = (await activityPubService.GetLocalIriFromUserIdentityAsync(userKey.Value)).Iri
                };

                var activityObject = Compositor.Composit(activityObjectDetails);

                try
                {
                    var (activityUri, objectUri) = await activityPubService.PublishActivityAsync(userKey.Value, ActivityType.Create, activityObject);
                }
                catch
                {
                    return await GetMessageErrorComponentAsOptionalAsync("failed to send activity");
                }

                return new(await componentFactory.GetPlainComponent(new TemporaryMessageComponentUpdateModel
                {
                    NotifySuccess = true,
                }));


                //var updaterComponent = await componentFactory.GetPlainComponent(new TemporaryMessageComponentUpdateModel
                //{
                //    AddMessages =
                //    [
                //        new() {
                //            Author = userKey.ToString(),
                //            Text = messageResult.Value,
                //            TimeStamp = DateTime.UtcNow
                //        },
                //        new() {
                //            Author = userKey.ToString(),
                //            Text = messageResult.Value,
                //            TimeStamp = DateTime.UtcNow
                //        },
                //        new() {
                //            Author = userKey.ToString(),
                //            Text = messageResult.Value,
                //            TimeStamp = DateTime.UtcNow
                //        }
                //    ]
                //});

                //if (!updaterComponent.IsSuccessful)
                //    return new(updaterComponent.Error);
                //return new(updaterComponent);
            }

            return new();
        }

        private async Task<Optional<IComponent>> GetLoginComponentAsOptionalAsync()
        {
            return new(await componentFactory.GetPlainComponent<LoginModel>(configureResponse: m => m.SetStatusCode = 401));
        }

        private async Task<Optional<IComponent>> GetMessageErrorComponentAsOptionalAsync(string errorMessage)
        {
            return new(await componentFactory.GetPlainComponent(new TemporaryMessageComponentUpdateModel
            {
                ErrorMessage = errorMessage
            }));
        }
    }
}
