using Elysium.Components.Components;
using Elysium.Core.Models;
using Elysium.Hosting.Services;
using Haondt.Web.Components;
using Haondt.Web.Core.Components;
using Haondt.Web.Core.Extensions;
using Haondt.Web.Core.Http;
using Haondt.Web.Core.Services;
using Microsoft.AspNetCore.Identity;

namespace Elysium.EventHandlers.Authentication
{
    public class LoginUserEventHandler(
        IHostingService hostingService,
        IComponentFactory componentFactory,
        SignInManager<UserIdentity> signInManager) : ISingleEventHandler
    {
        public string EventName => "LoginUser";

        public async Task<IComponent> HandleAsync(IRequestData requestData)
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
                return await componentFactory.GetPlainComponent(model);
            }

            var result = await signInManager.PasswordSignInAsync(
                localizedUsernameResult.Value,
                passwordResult.Value,
                isPersistent: true, lockoutOnFailure: false);

            if (!result.Succeeded)
                return await componentFactory.GetPlainComponent(new LoginModel
                {
                    Host = hostingService.Host,
                    ExistingLocalizedUsername = localizedUsernameResult.Value,
                    Errors = ["Incorrect username or password."]
                });

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
    }
}
