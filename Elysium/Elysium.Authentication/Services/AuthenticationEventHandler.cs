using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNext;
using Haondt.Identity.StorageKey;
using Haondt.Web;
using Haondt.Web.Core.Components;
using Haondt.Web.Core.Http;
using Haondt.Web.Core.Extensions;
using Haondt.Web.Services;
using Microsoft.AspNetCore.Identity;
using Elysium.Authentication.Exceptions;
using Elysium.Core.Models;
using Elysium.Components.Components;
using Haondt.Web.Components;
using Haondt.Web.Core.Services;

namespace Elysium.Authentication.Services
{
    public class AuthenticationEventHandler(
        UserManager<UserIdentity> userManager,
        SignInManager<UserIdentity> signInManager,
        IComponentFactory componentFactory) : IEventHandler
    {
        public const string REGISTER_USER_EVENT = "RegisterUser";
        public const string LOGIN_USER_EVENT = "LoginUser";
        public async Task<Result<Optional<IComponent>>> HandleAsync(string eventName, IRequestData requestData)
        {
            if (REGISTER_USER_EVENT.Equals(eventName))
            {
                var passwordResult = requestData.Form.GetValue<string>("password");
                var usernameResult = requestData.Form.GetValue<string>("username");
                if (!usernameResult.IsSuccessful || !passwordResult.IsSuccessful)
                {
                    var model = new RegisterModalModel();
                    if (usernameResult.IsSuccessful)
                        model.ExistingUsername = usernameResult.Value;
                    else
                    {
                        model.DangerUsername = true;
                        model.Errors.Add("Username is required.");
                    }
                    if (!passwordResult.IsSuccessful)
                    {
                        model.DangerPassword = true;
                        model.Errors.Add("Password is required.");
                    }
                    return await GetRegisterComponentAsync(model);
                }

                var user = new UserIdentity
                {
                    Id = UserIdentity.GetStorageKey(usernameResult.Value),
                    Username = usernameResult.Value,
                };
                var result = await userManager.CreateAsync(user, passwordResult.Value);

                if (!result.Succeeded)
                    return await GetRegisterComponentAsync(new RegisterModalModel
                    {
                        ExistingUsername = usernameResult.Value,
                        Errors = result.Errors.Select(e => $"{e.Description}").ToList()
                    });

                await signInManager.SignInAsync(user, isPersistent: true);
                return await GetHomeLoaderComponentAsync();
            }

            if (LOGIN_USER_EVENT.Equals(eventName))
            {
                var passwordResult = requestData.Form.GetValue<string>("password");
                var usernameResult = requestData.Form.GetValue<string>("username");
                if (!usernameResult.IsSuccessful || !passwordResult.IsSuccessful)
                {
                    var model = new LoginModel();
                    if (usernameResult.IsSuccessful)
                        model.ExistingUsername = usernameResult.Value;
                    else
                    {
                        model.DangerUsername = true;
                        model.Errors.Add("Username is required.");
                    }
                    if (!passwordResult.IsSuccessful)
                    {
                        model.DangerPassword = true;
                        model.Errors.Add("Password is required.");
                    }
                    return await GetLoginComponentAsync(model);
                }

                var result = await signInManager.PasswordSignInAsync(
                    usernameResult.Value,
                    passwordResult.Value,
                    isPersistent: true, lockoutOnFailure: false);

                if (!result.Succeeded)
                    return await GetLoginComponentAsync(new LoginModel
                    { 
                        ExistingUsername = usernameResult.Value,
                        Errors = ["Incorrect username or password."]
                    });

                return await GetHomeLoaderComponentAsync();
            }

            return new(Optional.Null<IComponent>());
        }

        public  async Task<Result<Optional<IComponent>>> GetHomeLoaderComponentAsync()
        {
            var closeModalComponent = await componentFactory.GetPlainComponent<CloseModalModel>();
            if (!closeModalComponent.IsSuccessful)
                return new(closeModalComponent.Error);

            var loaderComponent = await componentFactory.GetPlainComponent(new LoaderModel
            {
                Target = $"/_component/{ComponentDescriptor<HomeLayoutModel>.TypeIdentity}"
            });
            if (!loaderComponent.IsSuccessful)
                return new(loaderComponent.Error);

            var appendComponent = await componentFactory.GetPlainComponent(new AppendComponentLayoutModel
            {
                Components = [loaderComponent.Value, closeModalComponent.Value]
            }, configureResponse: m =>
            {
                m.ConfigureHeadersAction = new HxHeaderBuilder()
                    .ReSwap("innerHTML")
                    .ReTarget("#content")
                    .PushUrl("home")
                    .Build();
            });

            return new(appendComponent);
        }

        public  async Task<Result<Optional<IComponent>>> GetLoginComponentAsync(LoginModel model, int? statusCode = 401)
        {
            var loginComponent = statusCode.HasValue
                ? await componentFactory.GetPlainComponent(model, configureResponse: m => m.SetStatusCode = statusCode.Value)
                : await componentFactory.GetPlainComponent(model);
            return new(loginComponent);
        }
        public  async Task<Result<Optional<IComponent>>> GetRegisterComponentAsync(RegisterModalModel model, int? statusCode = 400)
        {
            var registerComponent = statusCode.HasValue
                ? await componentFactory.GetPlainComponent(model, configureResponse: m => m.SetStatusCode = statusCode.Value)
                : await componentFactory.GetPlainComponent(model);
            return new (registerComponent);
        }


    }
}
