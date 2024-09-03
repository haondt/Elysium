using DotNext;
using Elysium.Components.Components;
using Haondt.Web.Core.Components;
using Haondt.Web.Core.Exceptions;
using Haondt.Web.Core.Extensions;
using Haondt.Web.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Elysium.Services
{
    public class ElysiumExceptionActionResultFactory(ISingletonComponentFactory componentFactoryFactory) : IExceptionActionResultFactory
    {
        public async Task<Result<IActionResult>> CreateAsync(Exception exception, HttpContext context)
        {
            var result = exception switch
            {
                KeyNotFoundException => new ErrorModel { ErrorCode = 404, Message = "Not Found" },
                MissingComponentException => new ErrorModel { ErrorCode = 404, Message = "Not Found" },
                BadHttpRequestException => new ErrorModel { ErrorCode = 400, Message = "Bad Request" },
                _ => new ErrorModel { ErrorCode = 500, Message = "Elysium ran into an unrecoverable error." }
            };

            var component = await componentFactoryFactory.CreateComponentFactory().GetComponent(result, configureResponse: m => m.SetStatusCode = result.ErrorCode);

            if (!component.IsSuccessful)
                return new(component.Error);

            return Components.Extensions.ComponentExtensions.CreateView(component.Value, context.Response.AsResponseData());
        }
    }
}
