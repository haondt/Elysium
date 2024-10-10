using Elysium.Authentication.Components;
using Elysium.Components.Abstractions;
using Haondt.Web.Core.Components;
using Haondt.Web.Core.Services;

namespace Elysium.Components.Components
{
    public class TemporaryMessageComponentLayoutModel : IComponentModel
    {
        public required List<TemporaryMessageModel> Messages { get; set; }
    }

    public class TemporaryMessageModel
    {
        public required string Author { get; set; }
        public required string Text { get; set; }
        public required DateTime TimeStamp { get; set; }
    }

    public class TemporaryMessageComponentDescriptorFactory : IComponentDescriptorFactory
    {
        public IComponentDescriptor Create()
        {
            return new NeedsAuthorizationComponentDescriptor<TemporaryMessageComponentLayoutModel>(() => new TemporaryMessageComponentLayoutModel
            {
                Messages = []
            })
            {
                ViewPath = "~/Components/TemporaryMessageComponentLayout.cshtml",
                AuthorizationChecks = [ComponentAuthorizationCheck.IsAuthenticated],
                ConfigureResponse = new(m => m.ConfigureHeadersAction = new HxHeaderBuilder()
                    .ReTarget("#fill-content")
                    .ReSwap("innerHTML")
                    .PushUrl("/messages")
                    .Build())
            };
        }
    }
}
