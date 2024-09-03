using Elysium.Services;
using Haondt.Web.Core.Services;

namespace Elysium.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddElysiumServices(this IServiceCollection services)
        {
            services.AddSingleton<IExceptionActionResultFactory, ElysiumExceptionActionResultFactory>();
            return services;
        }
    }
}
