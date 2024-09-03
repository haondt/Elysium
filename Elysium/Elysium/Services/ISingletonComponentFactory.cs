using Haondt.Web.Core.Components;

namespace Elysium.Services
{
    public interface ISingletonComponentFactory
    {
        IComponentFactory CreateComponentFactory();
    }
}
