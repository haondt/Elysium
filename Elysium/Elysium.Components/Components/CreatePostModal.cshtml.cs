using Haondt.Web.Core.Components;

namespace Elysium.Components.Components
{
    public class CreatePostModalModel : IComponentModel
    {
        public bool HasEmptyContent { get; set; }
        public string? AudienceError { get; set; }
        public string? ExistingText { get; set; }
        public string? ExistingTitle { get; set; }
        public string? ExistingAudience { get; set; }
    }
}
