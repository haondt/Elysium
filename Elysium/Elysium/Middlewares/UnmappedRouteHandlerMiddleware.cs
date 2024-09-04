using Elysium.Components.Components;
using Elysium.Components.Extensions;
using Elysium.Services;
using Haondt.Web.Core.Extensions;
using Haondt.Web.Services;
using Microsoft.Extensions.Options;

namespace Elysium.Middlewares
{
    public class UnmappedRouteHandlerMiddleware(RequestDelegate next, ISingletonPageComponentFactory pageFactory, IOptions<ErrorSettings> errorOptions)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            await next(context);

            if (context.Response.StatusCode != StatusCodes.Status404NotFound)
                return;

            if (context.Request.Path.StartsWithSegments("/_"))
                return;

            if (context.Response.ContentLength != null)
                return;

            if (!string.IsNullOrEmpty(context.Response.ContentType))
                return;

            var message = errorOptions.Value.ShowErrorInfo
                ? $"The request url {context.Request.Path} was not found"
                : "404 Not Found";
            var component = await pageFactory.GetComponent<ErrorModel>(new Dictionary<string, string>
            {
                { "errorCode", "404" },
                { "message", message },
                { "title", "404 Not Found" }
            });

            if (!component.IsSuccessful)
                return;

            var viewResult = component.Value.CreateView(context.Response.AsResponseData());
            await viewResult.ExecuteResultAsync(new()
            {
                HttpContext = context,
                RouteData = context.GetRouteData(),
                ActionDescriptor = new()
            });
        }

    }
}
