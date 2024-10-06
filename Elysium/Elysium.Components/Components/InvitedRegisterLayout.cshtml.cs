using Haondt.Web.Core.Components;

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
}
