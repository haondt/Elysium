using Elysium.Authentication.Components;
using Elysium.Components.Abstractions;
using Haondt.Web.Core.Components;
using Haondt.Web.Core.Services;

namespace Elysium.Components.Components
{
    public class CreatePostModalModel : IComponentModel
    {
        public bool HasEmptyContent { get; set; }
        public string? AudienceError { get; set; }
        public string? ExistingText { get; set; }
        public string? ExistingTitle { get; set; }
        public string? ExistingAudience { get; set; }
    }

    public class CreatePostModalComponentDescriptorFactory : IComponentDescriptorFactory
    {
        public IComponentDescriptor Create()
        {
            return new NeedsAuthorizationComponentDescriptor<CreatePostModalModel>(() => new())
            {
                ViewPath = "~/Components/CreatePostModal.cshtml",
                AuthorizationChecks = [ComponentAuthorizationCheck.IsAuthenticated],
                ConfigureResponse = new(m => m.ConfigureHeadersAction = new HxHeaderBuilder()
                .ReSwap("none")
                .Build())
            };
        }
    }
}
