using Elysium.Components.Services;
using Haondt.Web.BulmaCSS.Extensions;
using Haondt.Web.Services;

namespace Elysium.Components.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddElysiumComponentServices(this IServiceCollection services)
        {
            services.AddSingleton<ISingletonComponentFactory, SingletonComponentFactory>();

            // subset of bulma css services
            services.AddBulmaCSSHeadEntries();
            services.AddBulmaCSSAssetSources();

            services.AddTransient<ILayoutUpdateFactory, LayoutUpdateFactory>();

            return services;
        }
    }
}
