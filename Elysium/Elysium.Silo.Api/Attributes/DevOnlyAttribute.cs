using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Elysium.Silo.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DevOnlyAttribute : Attribute, IFilterFactory
    {
        public bool IsReusable => true;

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            return ActivatorUtilities.CreateInstance<DevOnlyAttributeImplementation>(serviceProvider);
        }

        private class DevOnlyAttributeImplementation(IWebHostEnvironment env) : Attribute, IAuthorizationFilter
        {
            public void OnAuthorization(AuthorizationFilterContext context)
            {
                if (!env.IsDevelopment())
                    context.Result = new NotFoundResult();
            }
        }
    }
}
