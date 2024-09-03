using DotNext;
using Elysium.Authentication.Services;
using Elysium.Components.Components;
using Elysium.Services;
using Haondt.Web.Assets;
using Haondt.Web.Core.Components;
using Haondt.Web.Core.Services;
using Haondt.Web.Services;

namespace Elysium.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddElysiumServices(this IServiceCollection services)
        {
            services.AddSingleton<ISingletonComponentFactory, SingletonComponentFactory>();
            services.AddSingleton<IExceptionActionResultFactory, ElysiumExceptionActionResultFactory>();
            services.AddScoped<IComponentHandler, ElysiumComponentHandler>();

            var assemblyPrefix = typeof(ServiceCollectionExtensions).Assembly.GetName().Name;
            services.AddScoped<IHeadEntryDescriptor>(sp => new IconDescriptor
            {
                Uri = $"/_asset/{assemblyPrefix}.wwwroot.icon.ico"
            });

            return services;
        }

        public static IServiceCollection AddElysiumComponents(this IServiceCollection services)
        {
            services.AddScoped<IComponentDescriptor>(sp => new ComponentDescriptor<HomeLayoutModel>(async (cf, rd) =>
            {
                var session = sp.GetRequiredService<ISessionService>();
                if (!session.IsAuthenticated())
                    return new Result<HomeLayoutModel>(new UnauthorizedAccessException());

                var feed = await cf.GetComponent(new FeedModel());
                if (!feed.IsSuccessful)
                    return new Result<HomeLayoutModel>(feed.Error);
                return new(new HomeLayoutModel
                {
                    Feed = feed.Value
                });
            })
            {
                ViewPath = "~/Components/HomeLayout.cshtml",
            });
            services.AddScoped<IComponentDescriptor>(_ => new ComponentDescriptor<FeedModel>()
            {
                ViewPath = "~/Components/Feed.cshtml",
            });
            services.AddScoped<IComponentDescriptor>(_ => new ComponentDescriptor<ErrorModel>()
            {
                ViewPath = "~/Components/Error.cshtml",
                ConfigureResponse = new(m => m.ConfigureHeadersAction = new HxHeaderBuilder()
                    .ReTarget("#content")
                    .ReSwap("innerHTML")
                    .Build())
            });
            services.AddScoped<IComponentDescriptor>(_ => new ComponentDescriptor<LoginModel>(new LoginModel())
            {
                ViewPath = "~/Components/Login.cshtml",
                ConfigureResponse = new(m => m.ConfigureHeadersAction = new HxHeaderBuilder()
                    .ReTarget("#content")
                    .ReSwap("innerHTML")
                    .PushUrl("login")
                    .Build())
            });
            return services;
        }

        public static IServiceCollection AddElysiumAssetSources(this IServiceCollection services)
        {
            var assembly = typeof(ServiceCollectionExtensions).Assembly;
            services.AddSingleton<IAssetSource>(sp => new ManifestAssetSource(assembly));
            return services;
        }
    }
}
