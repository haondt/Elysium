using Elysium.Authentication.Exceptions;
using Elysium.Components.Components;
using Haondt.Web.Core.Components;
using Haondt.Web.Services;

namespace Elysium.Services
{
    public class ElysiumComponentHandler(IComponentFactory componentFactory) : IComponentHandler
    {
        public async Task<IComponent> HandleAsync(string componentIdentity)
        {
            try
            {
                return await componentFactory.GetComponent(componentIdentity);
            }
            catch (NeedsAuthenticationException)
            {
                return await componentFactory.GetPlainComponent<LoginModel>();
            }
        }
    }
}
