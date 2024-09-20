
using Haondt.Web.Core.Components;

namespace Elysium.Components.Components
{
    public class FeedModel : IComponentModel
    {
        public required List<IComponent<MediaModel>> Media { get; set; }
    }
}
