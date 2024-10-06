using Haondt.Web.Core.Components;

namespace Elysium.Authentication.Components
{
    public interface INeedsAuthorizationComponentDescriptor : IComponentDescriptor
    {
        public List<ComponentAuthorizationCheck> AuthorizationChecks { get; }
    }
}