using Haondt.Web.Core.Components;

namespace Elysium.Components
{
    public class HomeLayoutModel : IComponentModel
    {
        public required IComponent<FeedModel> Feed { get; set; }
    }
}
