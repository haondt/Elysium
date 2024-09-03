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
            var component = await componentFactory.GetComponent(componentIdentity);
            if (component.IsSuccessful)
                return component.Value;

            if (component.Error is UnauthorizedAccessException)
            {
                var loginComponent = await componentFactory.GetPlainComponent<LoginModel>();
                return loginComponent.Value;
            }

            return component.Value;
        }
    }
}
