using Elysium.Components.Abstractions;
using Haondt.Web.Core.Components;

namespace Elysium.Components.Components
{
    public class CloseModalModel : IComponentModel
    {
    }

    public class CloseModalComponentDescriptorFactory : IComponentDescriptorFactory
    {
        public IComponentDescriptor Create()
        {
            return new ComponentDescriptor<CloseModalModel>(new CloseModalModel())
            {
                ViewPath = "~/Components/CloseModal.cshtml"
            };
        }
    }
}
