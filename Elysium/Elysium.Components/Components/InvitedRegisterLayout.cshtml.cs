using Elysium.Components.Abstractions;
using Elysium.Hosting.Services;
using Haondt.Web.Core.Components;
using Haondt.Web.Core.Extensions;


namespace Elysium.Components.Components
{
    public class InvitedRegisterLayoutModel : IComponentModel
    {
        public bool DangerUsername { get; set; }
        public string? ExistingLocalizedUsername { get; set; }
        public bool DangerPassword { get; set; }
        public List<string> Errors { get; set; } = [];
        public required string InviteId { get; set; }
        public required string Host { get; set; }
    }

    public class InvitedRegisterLayoutComponentDescriptorFactory(IHostingService hostingService) : IComponentDescriptorFactory
    {
        public IComponentDescriptor Create()
        {
            return new ComponentDescriptor<InvitedRegisterLayoutModel>((cf, rd) =>
            {
                var host = hostingService.Host;

                var inviteId = rd.Query.TryGetValue<string>("inviteId");

                return new InvitedRegisterLayoutModel
                {
                    Host = host,
                    InviteId = inviteId.HasValue ? inviteId.Value : ""
                };
            })
            {
                ViewPath = "~/Components/InvitedRegisterLayout.cshtml",
            };
        }
    }
}
