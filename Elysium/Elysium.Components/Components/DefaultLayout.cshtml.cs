using Haondt.Web.Core.Components;
namespace Elysium.Components.Components
{
    public class DefaultLayoutModel : IComponentModel
    {
        public required IComponent Content { get; set; }
    }
}
