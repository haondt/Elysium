using Elysium.Components.Abstractions;
using Haondt.Core.Models;
using Haondt.Web.Core.Components;
using Haondt.Web.Core.Extensions;
using Haondt.Web.Core.Services;


namespace Elysium.Components.Components
{
    public class ErrorModel : IComponentModel
    {
        public required int ErrorCode { get; set; }
        public required string Message { get; set; }
        public Optional<string> Title { get; set; }
        public Optional<string> Details { get; set; }
    }

    public class ErrorComponentDescriptorFactory : IComponentDescriptorFactory
    {
        public IComponentDescriptor Create()
        {
            return new ComponentDescriptor<ErrorModel>((componentFactory, requestData) =>
            {
                var errorCode = requestData.Query.GetValue<int>("errorCode");
                var message = requestData.Query.GetValue<string>("message");
                var title = requestData.Query.TryGetValue<string>("title");
                var details = requestData.Query.TryGetValue<string>("details");
                return new ErrorModel
                {
                    ErrorCode = errorCode,
                    Message = message,
                    Title = title,
                    Details = details,
                };
            })
            {
                ViewPath = "~/Components/Error.cshtml",
                ConfigureResponse = new(m => m.ConfigureHeadersAction = new HxHeaderBuilder()
                    .ReTarget("#content")
                    .ReSwap("innerHTML")
                    .Build())
            };
        }
    }
}
