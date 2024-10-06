using Elysium.ActivityPub.Helpers.ActivityCompositor;
using Elysium.ActivityPub.Models;
using Elysium.Authentication.Services;
using Elysium.Client.Services;
using Elysium.Components.Components;
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
        public const string CREATE_POST_EVENT = "CreatePost";

        public async Task<Optional<IComponent>> HandleAsync(string eventName, IRequestData requestData)
        {
            if (CREATE_POST_EVENT.Equals(eventName))
            {
                if (!sessionService.IsAuthenticated())
                    return await GetLoginComponentAsOptionalAsync();

                var userKey = await sessionService.GetUserKeyAsync();
                if (!userKey.HasValue)
                    return await GetLoginComponentAsOptionalAsync();
                var userIri = await activityPubService.GetLocalIriFromUserIdentityAsync(userKey.Value);

                var titleResult = requestData.Form.TryGetValue<string>("title");
                var textResult = requestData.Form.TryGetValue<string>("text");
                var audienceResult = requestData.Form.TryGetValue<string>("audience");

                string? textValue = (textResult.HasValue && !(string.IsNullOrWhiteSpace(textResult.Value))) ? textResult.Value.Trim() : null;
                string? titleValue = (titleResult.HasValue && !(string.IsNullOrWhiteSpace(titleResult.Value))) ? titleResult.Value.Trim() : null;

                if (!audienceResult.HasValue)
                    return new(await componentFactory.GetPlainComponent(
                        new CreatePostModalModel
                        {
                            ExistingText = textValue,
                            ExistingTitle = titleValue,
                            AudienceError = "Please select an audience."
                        },
                        configureResponse: m => m.SetStatusCode = 400));

                // todo: audience targeting

                if ((!titleResult.HasValue || string.IsNullOrWhiteSpace(titleResult.Value)) && (!textResult.HasValue || string.IsNullOrWhiteSpace(textResult.Value)))
                    return new(await componentFactory.GetPlainComponent(
                        new CreatePostModalModel
                        {
                            HasEmptyContent = true,
                            ExistingAudience = audienceResult.Value
                        },
                        configureResponse: m => m.SetStatusCode = 400));

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
                return new(await componentFactory.GetPlainComponent<CloseModalModel>());
            }

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
