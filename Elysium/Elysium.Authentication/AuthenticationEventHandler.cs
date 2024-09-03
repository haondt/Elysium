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

namespace Elysium.Authentication
{
    public class AuthenticationEventHandler(
        UserManager<UserIdentity> userManager,
        SignInManager<UserIdentity> signInManager) : IEventHandler
    {
        public async Task<Result<Optional<IComponent>>> HandleAsync(string eventName, IRequestData requestData)
        {
            if (eventName == "RegisterUser")
            {
                var passwordResult = requestData.Form.GetValue<string>("password");
                if (!passwordResult.IsSuccessful)
                    return new(passwordResult.Error);

                var usernameResult = requestData.Form.GetValue<string>("username");
                if (!usernameResult.IsSuccessful)
                    return new(usernameResult.Error);

                var user = new UserIdentity
                {
                    Id = UserIdentity.GetStorageKey(usernameResult.Value),
                    Username = usernameResult.Value,
                };
                var result = await userManager.CreateAsync(user, passwordResult.Value);

                if (!result.Succeeded)
                    return new(new IdentityErrorsException(result.Errors));

                await signInManager.SignInAsync(user, isPersistent: true);
            }

            return new(Optional.Null<IComponent>());
        }
    }
}
