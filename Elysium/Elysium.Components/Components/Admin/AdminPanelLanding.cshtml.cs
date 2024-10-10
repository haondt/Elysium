using Elysium.Authentication.Components;
using Elysium.Components.Abstractions;
using Haondt.Web.Core.Components;

namespace Elysium.Components.Components.Admin
{
    public class AdminPanelLandingModel : IComponentModel
    {
    }

    public class AdminPanelLandingComponentDescriptorFactory : IComponentDescriptorFactory
    {
        public IComponentDescriptor Create()
        {
            return new NeedsAuthorizationComponentDescriptor<AdminPanelLandingModel>(new AdminPanelLandingModel())
            {
                ViewPath = "~/Components/Admin/AdminPanelLanding.cshtml",
                AuthorizationChecks = [ComponentAuthorizationCheck.IsAdministrator],
            };
        }
    }
}
