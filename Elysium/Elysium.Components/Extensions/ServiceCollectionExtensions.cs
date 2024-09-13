using Elysium.Components.Services;

namespace Elysium.Components.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddElysiumComponentServices(this IServiceCollection services)
        {
            services.AddSingleton<ISingletonComponentFactory, SingletonComponentFactory>();
            return services;
        }

    }
}
