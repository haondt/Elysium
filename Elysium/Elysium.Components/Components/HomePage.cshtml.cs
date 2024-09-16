using Haondt.Web.Core.Components;

namespace Elysium.Components.Components
{
    public class HomePageModel : IComponentModel
    {
        public required IComponent<ShadeSelectorModel> ShadeSelector { get; set; }
        public required IComponent<FeedModel> Feed { get; set; }
    }
}
