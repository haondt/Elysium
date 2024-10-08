using Elysium.Authentication.Components;
using Elysium.Components.Abstractions;
using Haondt.Core.Models;
using Haondt.Web.Core.Components;

namespace Elysium.Components.Components
{
    public class TemporaryMessageComponentUpdateModel : IComponentModel
    {
        public bool ClearMessage { get; set; }
        public bool ClearReceiver { get; set; }
        public Optional<string> ErrorMessage { get; set; }
        public bool NotifySuccess { get; set; }
        public List<TemporaryMessageModel> AddMessages { get; set; } = [];
    }

    public class TemporaryMessageUpdateComponentDescriptorFactory : IComponentDescriptorFactory
    {
        public IComponentDescriptor Create()
        {
            return new NeedsAuthorizationComponentDescriptor<TemporaryMessageComponentUpdateModel>()
            {
                ViewPath = "~/Components/TemporaryMessageComponentUpdate.cshtml",
                AuthorizationChecks = [ComponentAuthorizationCheck.IsAuthenticated],
            };
        }
    }
}
