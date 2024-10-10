
using Elysium.Client.Services;
using Elysium.Components.Components;
using Elysium.Core.Models;
using Elysium.Hosting.Services;
using Haondt.Core.Models;
using Haondt.Web.Components;
using Haondt.Web.Core.Components;
using Haondt.Web.Core.Extensions;
using Haondt.Web.Core.Http;
using Haondt.Web.Core.Services;
using Microsoft.AspNetCore.Identity;
using System.Runtime.ExceptionServices;

namespace Elysium.EventHandlers.Authentication
{
    public class InviteStateAgnosticRegisterUserEventHandler(
        IComponentFactory componentFactory,
        IHostingService hostingService,
        IElysiumService elysiumService,
        SignInManager<UserIdentity> signInManager)
    {

        public async Task<(bool CreatedAccount, Result<IComponent, ExceptionDispatchInfo> Result)> HandleAsync(IRequestData requestData)
        {
            bool createdAccount = false;
            try
            {
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
                    return (false, new(await componentFactory.GetPlainComponent(model)));
                }

                var registrationResult = await elysiumService.RegisterUserAsync(
                    localizedUsernameResult.Value,
                    passwordResult.Value);
                if (!registrationResult.IsSuccessful)
                    return (false, new(await componentFactory.GetPlainComponent(new RegisterModalModel
                    {
                        ExistingLocalizedUsername = localizedUsernameResult.Value,
                        Host = hostingService.Host,
                        Errors = registrationResult.Reason
                    })));

                createdAccount = true;

                await signInManager.SignInAsync(registrationResult.Value, isPersistent: true);

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

                return (true, new(appendComponent));
            }
            catch (Exception ex)
            {
                return (createdAccount, new(ExceptionDispatchInfo.Capture(ex)));
            }
        }
    }
}
