using DotNext;
using Elysium.ActivityPub;
using Elysium.Authentication.Services;
using Elysium.Client.Services;
using Elysium.Components.Components;
using Elysium.Server.Services;
using Haondt.Web.Core.Components;
using Haondt.Web.Core.Extensions;
using Haondt.Web.Core.Http;
using Haondt.Web.Services;

namespace Elysium.Services
{
    public class ElysiumPublishActivityEventHandler(
        IActivityPubClientService activityPubService,
        IHostingService hostingService,
        ISessionService sessionService,
        IComponentFactory componentFactory) : IEventHandler
    {
        public const string SEND_MESSAGE_EVENT = "SendMessage";

        public async Task<Result<Optional<IComponent>>> HandleAsync(string eventName, IRequestData requestData)
        {

            if (SEND_MESSAGE_EVENT.Equals(eventName))
            {
                if (!sessionService.IsAuthenticated())
                    return await GetLoginComponentAsync();

                var userKey = await sessionService.GetUserKeyAsync();
                if (!userKey.IsSuccessful)
                    return await GetLoginComponentAsync();

                var messageResult = requestData.Form.GetValue<string>("message");
                if (!messageResult.IsSuccessful)
                    return await GetMessageErrorComponentAsync(messageResult.Error.Message);
                var recepientResult = requestData.Form.GetValue<string>("recepient");
                if (!recepientResult.IsSuccessful)
                    return await GetMessageErrorComponentAsync(recepientResult.Error.Message);
                var recepientUri = await hostingService.GetUriForUsernameAsync(recepientResult.Value);
                if (!recepientUri.IsSuccessful)
                    return await GetMessageErrorComponentAsync(recepientUri.Error.Message);

                var activityObjectDetails = new MessageDetails
                {
                    Text = messageResult.Value,
                    Recepient = recepientUri.Value
                };

                var activityObject = ActivityCompositor.Composit(activityObjectDetails);
                if (!activityObject.IsSuccessful)
                {
                    return await GetMessageErrorComponentAsync(activityObject.Error.Message);
                } 

                return new(await componentFactory.GetPlainComponent(new TemporaryMessageComponentUpdateModel
                {
                    NotifySuccess = true,
                }));
                //activityPubService.PublishActivityAsync(userKey.Value, ActivityType.Create,  );


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

            return new(Optional.Null<IComponent>());
        }

        private async Task<Result<Optional<IComponent>>> GetLoginComponentAsync()
        {
            return new(await componentFactory.GetPlainComponent<LoginModel>(configureResponse: m => m.SetStatusCode = 401));
        }

        private async Task<Result<Optional<IComponent>>> GetMessageErrorComponentAsync(string errorMessage)
        {
            return new(await componentFactory.GetPlainComponent(new TemporaryMessageComponentUpdateModel
            {
                ErrorMessage = errorMessage
            }));
        }
    }
}
