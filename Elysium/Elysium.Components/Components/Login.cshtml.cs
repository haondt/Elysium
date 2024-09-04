using Haondt.Web.Core.Components;

namespace Elysium.Components.Components
{
    public class LoginModel : IComponentModel
    {
        public string? ExistingUsername { get; set; }
        public List<string> Errors { get; set; } = [];
        public bool DangerUsername { get; set; } = false;
        public bool DangerPassword { get; set; } = false;
    }
}
