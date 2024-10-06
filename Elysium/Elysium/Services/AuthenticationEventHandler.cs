using Elysium.Authentication.Exceptions;
using Elysium.Authentication.Services;
using Elysium.Client.Services;
using Elysium.Components.Components;
using Elysium.Components.Components.Admin;
using Elysium.Core.Models;
using Elysium.Hosting.Services;
using Elysium.Persistence.Services;
using Haondt.Core.Models;
using Haondt.Web.Components;
using Haondt.Web.Core.Components;
using Haondt.Web.Core.Extensions;
using Haondt.Web.Core.Http;
using Haondt.Web.Core.Services;
using Haondt.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Elysium.Services
{
    public class AuthenticationEventHandler(
        SignInManager<UserIdentity> signInManager,
        IComponentFactory componentFactory,
        IElysiumService elysiumService,
        IOptions<RegistrationSettings> registrationOptions,
        IElysiumStorage storage,
        ISessionService sessionService,
        IHostingService hostingService) : IEventHandler
    {
        public const string REGISTER_USER_EVENT = "RegisterUser";
        public const string LOGIN_USER_EVENT = "LoginUser";
        public const string GENERATE_INVITE_LINK_EVENT = "GenerateInvite";
        public const string INVITED_REGISTER_USER_EVENT = "InvitedRegisterUser";
        public async Task<Optional<IComponent>> HandleAsync(string eventName, IRequestData requestData)
        {
            if (INVITED_REGISTER_USER_EVENT.Equals(eventName))
            {
                var inviteIdResult = requestData.Form.TryGetValue<string>("inviteId");
                if (!inviteIdResult.HasValue)
                    return new(await GetInvitedRegisterComponentAsync(new InvitedRegisterLayoutModel
                    {
                        Host = hostingService.Host,
                        Errors = ["Invalid invite link"],
                        InviteId = ""
                    }));

                var inviteKeyToDelete = InviteLinkDetails.GetStorageKey(inviteIdResult.Value);
                var invite = await storage.Get(inviteKeyToDelete);
                if (!invite.IsSuccessful)
                    return new(await GetInvitedRegisterComponentAsync(new InvitedRegisterLayoutModel
                    {
                        Host = hostingService.Host,
                        Errors = ["Invalid invite link"],
                        InviteId = ""
                    }));

                // todo: other validations on invite link

                var passwordResult = requestData.Form.TryGetValue<string>("password");
                var localizedUsernameResult = requestData.Form.TryGetValue<string>("localizedUsername");
                if (!localizedUsernameResult.HasValue || !passwordResult.HasValue)
                {
                    var model = new RegisterModalModel { Host = hostingService.Host };
                    if (localizedUsernameResult.HasValue)
                        model.ExistingLocalizedUsername = localizedUsernameResult.Value;
                    else
                    {
                        model.DangerUsername = true;
                        model.Errors.Add("Username is required.");
                    }
                    if (!passwordResult.HasValue)
                    {
                        model.DangerPassword = true;
                        model.Errors.Add("Password is required.");
                    }
                    return new(await GetRegisterComponentAsync(model));
                }

                var registrationResult = await elysiumService.RegisterUserAsync(
                    localizedUsernameResult.Value,
                    passwordResult.Value);
                if (!registrationResult.IsSuccessful)
                    return new(await GetRegisterComponentAsync(new RegisterModalModel
                    {
                        ExistingLocalizedUsername = localizedUsernameResult.Value,
                        Host = hostingService.Host,
                        Errors = registrationResult.Reason
                    }));

                await storage.Delete(inviteKeyToDelete);
                await signInManager.SignInAsync(registrationResult.Value, isPersistent: true);
                return new(await GetHomeLoaderComponentAsync());
            }

            if (REGISTER_USER_EVENT.Equals(eventName))
            {
                if (registrationOptions.Value.InviteOnly)
                    return new(await GetRegisterComponentAsync(new RegisterModalModel
                    {
                        Host = hostingService.Host,
                        Errors = ["This server is invite-only."],
                    }));

                var passwordResult = requestData.Form.TryGetValue<string>("password");
                var localizedUsernameResult = requestData.Form.TryGetValue<string>("localizedUsername");
                if (!localizedUsernameResult.HasValue || !passwordResult.HasValue)
                {
                    var model = new RegisterModalModel { Host = hostingService.Host };
                    if (localizedUsernameResult.HasValue)
                        model.ExistingLocalizedUsername = localizedUsernameResult.Value;
                    else
                    {
                        model.DangerUsername = true;
                        model.Errors.Add("Username is required.");
                    }
                    if (!passwordResult.HasValue)
                    {
                        model.DangerPassword = true;
                        model.Errors.Add("Password is required.");
                    }
                    return new(await GetRegisterComponentAsync(model));
                }

                var registrationResult = await elysiumService.RegisterUserAsync(
                    localizedUsernameResult.Value,
                    passwordResult.Value);
                if (!registrationResult.IsSuccessful)
                    return new(await GetRegisterComponentAsync(new RegisterModalModel
                    {
                        ExistingLocalizedUsername = localizedUsernameResult.Value,
                        Host = hostingService.Host,
                        Errors = registrationResult.Reason
                    }));

                await signInManager.SignInAsync(registrationResult.Value, isPersistent: true);
                return new(await GetHomeLoaderComponentAsync());
            }

            if (LOGIN_USER_EVENT.Equals(eventName))
            {
                var passwordResult = requestData.Form.TryGetValue<string>("password");
                var localizedUsernameResult = requestData.Form.TryGetValue<string>("localizedUsername");
                if (!localizedUsernameResult.HasValue || !passwordResult.HasValue)
                {
                    var model = new LoginModel { Host = hostingService.Host };
                    if (localizedUsernameResult.HasValue)
                        model.ExistingLocalizedUsername = localizedUsernameResult.Value;
                    else
                    {
                        model.DangerUsername = true;
                        model.Errors.Add("Username is required.");
                    }
                    if (!passwordResult.HasValue)
                    {
                        model.DangerPassword = true;
                        model.Errors.Add("Password is required.");
                    }
                    return new(await GetLoginComponentAsync(model));
                }

                var result = await signInManager.PasswordSignInAsync(
                    localizedUsernameResult.Value,
                    passwordResult.Value,
                    isPersistent: true, lockoutOnFailure: false);

                if (!result.Succeeded)
                    return new(await GetLoginComponentAsync(new LoginModel
                    {
                        Host = hostingService.Host,
                        ExistingLocalizedUsername = localizedUsernameResult.Value,
                        Errors = ["Incorrect username or password."]
                    }));

                return new(await GetHomeLoaderComponentAsync());
            }

            if (GENERATE_INVITE_LINK_EVENT.Equals(eventName))
            {
                if (!sessionService.IsAdministrator())
                    throw new NeedsAuthorizationException();

                var link = await elysiumService.GenerateInviteLinkAsync();
                return new Optional<IComponent>(await componentFactory.GetPlainComponent(new GenerateInviteModel
                {
                    InviteLink = link
                }));
            }

            return new();
        }

        public async Task<IComponent> GetHomeLoaderComponentAsync()
        {
            var closeModalComponent = await componentFactory.GetPlainComponent<CloseModalModel>();
            var loaderComponent = await componentFactory.GetPlainComponent(new LoaderModel
            {
                Target = $"/_component/{ComponentDescriptor<HomePageModel>.TypeIdentity}"
            });
            var appendComponent = await componentFactory.GetPlainComponent(new AppendComponentLayoutModel
            {
                Components = [loaderComponent, closeModalComponent]
            }, configureResponse: m =>
            {
                m.ConfigureHeadersAction = new HxHeaderBuilder()
                    .ReSwap("innerHTML")
                    .ReTarget("#content")
                    .PushUrl("/home")
                    .Build();
            });

            return appendComponent;
        }

        public async Task<IComponent> GetLoginComponentAsync(LoginModel model, int? statusCode = 401)
        {
            if (statusCode.HasValue)
                return await componentFactory.GetPlainComponent(model, configureResponse: m => m.SetStatusCode = statusCode.Value);
            return await componentFactory.GetPlainComponent(model);
        }

        public async Task<IComponent> GetRegisterComponentAsync(RegisterModalModel model, int? statusCode = 400)
        {
            if (statusCode.HasValue)
                return await componentFactory.GetPlainComponent(model, configureResponse: m => m.SetStatusCode = statusCode.Value);
            return await componentFactory.GetPlainComponent(model);
        }
        public async Task<IComponent> GetInvitedRegisterComponentAsync(InvitedRegisterLayoutModel model, int? statusCode = 400)
        {
            if (statusCode.HasValue)
                return await componentFactory.GetPlainComponent(model, configureResponse: m => m.SetStatusCode = statusCode.Value);
            return await componentFactory.GetPlainComponent(model);
        }
    }
}
