using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Elysium.Domain.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddElysiumGrainServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<HostIntegritySettings>(configuration.GetSection(nameof(HostIntegritySettings)));
            services.Configure<InstanceActorSettings>(configuration.GetSection(nameof(InstanceActorSettings)));
            services.AddTransient<IMemoryCache, MemoryCache>();

            return services;
        }
    }
}
