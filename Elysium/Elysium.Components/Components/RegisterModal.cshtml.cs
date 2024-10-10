using Elysium.Components.Abstractions;
using Elysium.Hosting.Services;
using Haondt.Web.Core.Components;
using Haondt.Web.Core.Services;

namespace Elysium.Components.Components
{
    public class RegisterModalModel : IComponentModel
    {
        public required string Host { get; set; }
        public string? ExistingLocalizedUsername { get; set; }
        public List<string> Errors { get; set; } = [];
        public bool DangerUsername { get; set; } = false;
        public bool DangerPassword { get; set; } = false;
    }
    public class RegisterModalComponentDescriptorFactory(IHostingService hostingService) : IComponentDescriptorFactory
    {
        public IComponentDescriptor Create()
        {
            return new ComponentDescriptor<RegisterModalModel>(() =>
            {
                var host = hostingService.Host;

                return new RegisterModalModel
                {
                    Host = host
                };
            })
            {
                ViewPath = "~/Components/RegisterModal.cshtml",
                ConfigureResponse = new(m => m.ConfigureHeadersAction = new HxHeaderBuilder()
                    .ReSwap("none")
                    .Build())
            };
        }
    }
}
