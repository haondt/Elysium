using Haondt.Web.Core.Components;

namespace Elysium.Components.Components
{
    public class HomeLayoutModel : IComponentModel
    {
        public required IComponent<FeedModel> Feed { get; set; }
    }
}
