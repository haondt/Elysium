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
            }

            if (LOGIN_USER_EVENT.Equals(eventName))
            {
                var passwordResult = requestData.Form.GetValue<string>("password");
                if (!passwordResult.IsSuccessful)
                    return new(passwordResult.Error);

                var usernameResult = requestData.Form.GetValue<string>("username");
                if (!usernameResult.IsSuccessful)
                    return new(usernameResult.Error);

                var result = await signInManager.PasswordSignInAsync(
                    usernameResult.Value,
                    passwordResult.Value,
                    isPersistent: true, lockoutOnFailure: false);

                if (!result.Succeeded)
                    return await GetLoginComponentAsync();

                var homePage = await componentFactory.GetPlainComponent<HomeLayoutModel>();
                if (homePage.IsSuccessful)
                    return new(new Optional<IComponent>(homePage.Value));
                return new(homePage.Error);
            }

            return new(Optional.Null<IComponent>());
        }

        public  async Task<Result<Optional<IComponent>>> GetLoginComponentAsync()
        {
           var result = await componentFactory.GetPlainComponent<LoginModel>(configureResponse: m => m.SetStatusCode = 401);
            if (!result.IsSuccessful)
                return new(result.Error);
            return new(new Optional<IComponent>(result.Value));
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
