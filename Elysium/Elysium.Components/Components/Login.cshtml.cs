using Elysium.Components.Abstractions;
using Elysium.Hosting.Services;
using Haondt.Web.Core.Components;
using Haondt.Web.Core.Services;

namespace Elysium.Components.Components
{
    public class LoginModel : IComponentModel
    {
        public required string Host { get; set; }
        public string? ExistingLocalizedUsername { get; set; }
        public List<string> Errors { get; set; } = [];
        public bool DangerUsername { get; set; } = false;
        public bool DangerPassword { get; set; } = false;
    }

    public class LoginComponentDescriptorFactory(IHostingService hostingService) : IComponentDescriptorFactory
    {
        public IComponentDescriptor Create()
        {
            return new ComponentDescriptor<LoginModel>(() =>
            {
                var host = hostingService.Host;

                return new LoginModel
                {
                    Host = host
                };
            })
            {
                ViewPath = "~/Components/Login.cshtml",
                ConfigureResponse = new(m => m.ConfigureHeadersAction = new HxHeaderBuilder()
                    .ReTarget("#content")
                    .ReSwap("innerHTML")
                    .PushUrl("/identity/login")
                    .Build())
            };
        }
    }
}
