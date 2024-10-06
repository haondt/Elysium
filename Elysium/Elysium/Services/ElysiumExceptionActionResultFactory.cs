using Elysium.Authentication.Exceptions;
using Elysium.Components.Components;
using Elysium.Components.Services;
using Haondt.Web.Components;
using Haondt.Web.Core.Exceptions;
using Haondt.Web.Core.Extensions;
using Haondt.Web.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Elysium.Services
{
    public class ElysiumExceptionActionResultFactory(ISingletonComponentFactory componentFactoryFactory, IOptions<ErrorSettings> errorOptions) : IExceptionActionResultFactory
    {
        public async Task<IActionResult> CreateAsync(Exception exception, HttpContext context)
        {
            var result = exception switch
            {
                KeyNotFoundException => new ErrorModel { ErrorCode = 404, Message = "Not Found" },
                MissingComponentException => new ErrorModel { ErrorCode = 404, Message = "Not Found" },
                BadHttpRequestException => new ErrorModel { ErrorCode = 400, Message = "Bad Request" },
                NeedsAuthorizationException => new ErrorModel { ErrorCode = 403, Message = "Forbidden" },
                _ => new ErrorModel { ErrorCode = 500, Message = "Elysium ran into an unrecoverable error." }
            };

            if (errorOptions.Value.ShowErrorInfo)
                result.Details = exception.ToString();

            var componentFactory = componentFactoryFactory.CreateComponentFactory();

            var errorComponent = await componentFactory.GetPlainComponent(result, configureResponse: m => m.SetStatusCode = result.ErrorCode);
            var closeModalComponent = await componentFactory.GetPlainComponent<CloseModalModel>();

            var appendComponent = await componentFactory.GetComponent(new AppendComponentLayoutModel
            {
                Components = [errorComponent, closeModalComponent]
            }, configureResponse: m =>
            {
                m.SetStatusCode = result.ErrorCode;
                m.ConfigureHeadersAction = new HxHeaderBuilder()
                    .ReSwap("innerHTML")
                    .ReTarget("#content")
                    .Build();
            });

            return Components.Extensions.ComponentExtensions.CreateView(appendComponent, context.Response.AsResponseData());
        }
    }
}
