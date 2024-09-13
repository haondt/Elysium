using Haondt.Web.Core.Components;

namespace Elysium.Components.Services
{
    public interface ISingletonComponentFactory
    {
        IComponentFactory CreateComponentFactory();
    }
}
