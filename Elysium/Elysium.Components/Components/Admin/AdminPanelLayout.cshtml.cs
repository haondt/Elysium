using Elysium.Authentication.Components;
using Elysium.Components.Abstractions;
using Haondt.Web.Core.Components;
using Haondt.Web.Core.Services;

namespace Elysium.Components.Components.Admin
{
    public class AdminPanelLayoutModel : IComponentModel
    {
        public required IComponent ActivePage { get; set; }

        public List<AdminPanelMenuItem> MenuItems => new()
        {
            new AdminPanelMenuItem
            {
                Name = "Access",
                Children = new List<AdminPanelMenuChild>
                {
                    new() {
                        Name = "Generate Invite",
                        ComponentIdentity = ComponentDescriptor<GenerateInviteModel>.TypeIdentity,
                        IsActive = ActivePage.Model is GenerateInviteModel
                    }
                }
            }
        };
    }

    public class AdminPanelMenuItem
    {
        public required string Name { get; set; }
        public List<AdminPanelMenuChild> Children { get; set; } = [];
    }

    public class AdminPanelMenuChild
    {
        public required string Name { get; set; }
        public required bool IsActive { get; set; }
        public required string ComponentIdentity { get; set; }
        //public List<AdminPanelMenuChild> Children { get; set; } = []; // todo maybe if we need it
    }

    public class AdminPanelLayoutComponentDescriptorFactory : IComponentDescriptorFactory
    {
        public IComponentDescriptor Create()
        {
            return new NeedsAuthorizationComponentDescriptor<AdminPanelLayoutModel>(async (componentFactory, requestData) =>
            {
                return new AdminPanelLayoutModel
                {
                    ActivePage = await componentFactory.GetPlainComponent<AdminPanelLandingModel>(),
                };
            })
            {
                ViewPath = "~/Components/Admin/AdminPanelLayout.cshtml",
                AuthorizationChecks = [ComponentAuthorizationCheck.IsAdministrator],
                ConfigureResponse = new(m => m.ConfigureHeadersAction = new HxHeaderBuilder()
                    .ReTarget("#content")
                    .ReSwap("innerHTML")
                    .PushUrl("/admin")
                    .Build())
            };
        }
    }
}
