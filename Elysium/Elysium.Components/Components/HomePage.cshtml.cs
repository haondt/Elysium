using Elysium.Authentication.Components;
using Elysium.Components.Abstractions;
using Haondt.Web.Core.Components;

namespace Elysium.Components.Components
{
    public class HomePageModel : IComponentModel
    {
        public required IComponent<ShadeSelectorModel> ShadeSelector { get; set; }
        public required IComponent<FeedModel> Feed { get; set; }
    }

    public class HomePageComponentDescriptorFactory : IComponentDescriptorFactory
    {
        public IComponentDescriptor Create()
        {
            return new NeedsAuthorizationComponentDescriptor<HomePageModel>(async (cf) =>
            {
                var feed = await cf.GetComponent<FeedModel>();
                var shadeSelector = await cf.GetComponent<ShadeSelectorModel>();
                return new HomePageModel
                {
                    ShadeSelector = shadeSelector,
                    Feed = feed
                };
            })
            {
                ViewPath = "~/Components/HomePage.cshtml",
                AuthorizationChecks = [ComponentAuthorizationCheck.IsAuthenticated]
            };
        }
    }
}
