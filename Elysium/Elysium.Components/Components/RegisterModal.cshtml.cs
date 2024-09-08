using Haondt.Web.Core.Components;

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
}
