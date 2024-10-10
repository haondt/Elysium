using Elysium.Components.Abstractions;
using Haondt.Web.Core.Components;
namespace Elysium.Components.Components
{
    public class DefaultLayoutModel : IComponentModel
    {
        public required IComponent Content { get; set; }
    }

    public class DefaultLayoutComponentDescriptorFactory : IComponentDescriptorFactory
    {
        public IComponentDescriptor Create()
        {
            return new ComponentDescriptor<DefaultLayoutModel>
            {
                ViewPath = $"~/Components/DefaultLayout.cshtml",
            };
        }
    }
}
