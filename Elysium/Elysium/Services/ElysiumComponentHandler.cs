using Elysium.Components.Components;
using Haondt.Web.Core.Components;
using Haondt.Web.Core.Services;
using Haondt.Web.Services;

namespace Elysium.Services
{
    public class ElysiumComponentHandler(IComponentFactory componentFactory) : IComponentHandler
    {
        public async Task<IComponent> HandleAsync(string componentIdentity)
        {
            try
            {
                var component = await componentFactory.GetComponent(componentIdentity);
                return component;
            }
            catch (UnauthorizedAccessException)
            {
                return await componentFactory.GetPlainComponent<LoginModel>();
            }
        }
    }
}
