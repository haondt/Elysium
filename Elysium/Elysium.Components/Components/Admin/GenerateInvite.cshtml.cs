using Elysium.Authentication.Components;
using Elysium.Components.Abstractions;
using Haondt.Web.Core.Components;
using Haondt.Web.Core.Services;

namespace Elysium.Components.Components.Admin
{
    public class GenerateInviteModel : IComponentModel
    {
        public string InviteLink { get; set; } = "";
    }

    public class GenerateInviteComponentDescriptorFactory : IComponentDescriptorFactory
    {
        public IComponentDescriptor Create()
        {
            return new NeedsAuthorizationComponentDescriptor<GenerateInviteModel>(new GenerateInviteModel())
            {
                ViewPath = "~/Components/Admin/GenerateInvite.cshtml",
                AuthorizationChecks = [ComponentAuthorizationCheck.IsAdministrator],
                ConfigureResponse = new(m => m.ConfigureHeadersAction = new HxHeaderBuilder()
                    .ReTarget("#admin-panel-page")
                    .ReSwap("innerHTML")
                    .Build())
            };
        }
    }
}
