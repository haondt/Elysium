using Elysium.Authentication.Services;
using Elysium.Components.Components;
using Elysium.Services;
using Haondt.Web.Assets;
using Haondt.Web.Core.Components;
using Haondt.Web.Core.Http;
using Haondt.Web.Core.Services;
using Haondt.Web.Services;
using Haondt.Web.Core.Extensions;
using Haondt.Web.Components;
using Elysium.Server.Services;
using Elysium.Authentication.Components;

namespace Elysium.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddElysiumServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ErrorSettings>(configuration.GetSection(nameof(ErrorSettings)));
            services.AddScoped<IEventHandler, ElysiumPublishActivityEventHandler>(); 
            services.AddSingleton<ISingletonComponentFactory, SingletonComponentFactory>();
            services.AddSingleton<ISingletonPageComponentFactory, SingletonPageComponentFactory>();
            services.AddScoped<IEventHandler, AuthenticationEventHandler>();
            services.AddSingleton<IExceptionActionResultFactory, ElysiumExceptionActionResultFactory>();
            services.AddScoped<IComponentHandler, ElysiumComponentHandler>();

            var assemblyPrefix = typeof(ServiceCollectionExtensions).Assembly.GetName().Name;
            services.AddScoped<IHeadEntryDescriptor>(sp => new IconDescriptor
            {
                Uri = $"/_asset/{assemblyPrefix}.wwwroot.icon.ico"
            });
            services.AddScoped<IHeadEntryDescriptor>(sp => new StyleSheetDescriptor
            {
                Uri = $"/_asset/{assemblyPrefix}.wwwroot.bulma-custom.css"
            });
            services.AddScoped<IHeadEntryDescriptor>(sp => new ScriptDescriptor
            {
                Uri = "https://kit.fontawesome.com/afd44816da.js",
                CrossOrigin = "anonymous"
            });
            services.AddScoped<IHeadEntryDescriptor>(_ => new MetaDescriptor
            {
                Name = "htmx-config",
                Content = @"{
                    ""responseHandling"": [
                        { ""code"": ""204"", ""swap"": false },
                        { ""code"": "".*"", ""swap"": true }
                    ]
                }",
            });
            return services;
        }

        public static IServiceCollection AddElysiumComponents(this IServiceCollection services)
        {
            services.AddScoped<IComponentDescriptor>(sp => new NeedsAuthenticationComponentDescriptor<HomeLayoutModel>(async (cf) =>
            {
                var feed = await cf.GetComponent(new FeedModel());
                return new HomeLayoutModel
                {
                    Feed = feed
                };
            })
            {
                ViewPath = "~/Components/HomeLayout.cshtml",
            });
            services.AddScoped<IComponentDescriptor>(_ => new ComponentDescriptor<FeedModel>()
            {
                ViewPath = "~/Components/Feed.cshtml",
            });
            services.AddScoped<IComponentDescriptor>(_ => new ComponentDescriptor<ErrorModel>((componentFactory, requestData) =>
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
            });
            services.AddScoped<IComponentDescriptor>(sp => new ComponentDescriptor<LoginModel>(() =>
            {
                var hostingService = sp.GetRequiredService<IHostingService>();
                var host = hostingService.Host;

                return new LoginModel
                {
                    Host = host
                };
            })
            {
                ViewPath = "~/Components/Login.cshtml",
                ConfigureResponse = new(m => m.ConfigureHeadersAction = new HxHeaderBuilder()
                    .ReTarget("#content")
                    .ReSwap("innerHTML")
                    .PushUrl("/identity/login")
                    .Build())
            });
            services.AddScoped<IComponentDescriptor>(_ => new ComponentDescriptor<CloseModalModel>(new CloseModalModel())
            {
                ViewPath = "~/Components/CloseModal.cshtml"
            });
            services.AddScoped<IComponentDescriptor>(sp => new ComponentDescriptor<RegisterModalModel>(() =>
            {
                var hostingService = sp.GetRequiredService<IHostingService>();
                var host = hostingService.Host;

                return new RegisterModalModel
                {
                    Host = host
                };
            })
            {
                ViewPath = "~/Components/RegisterModal.cshtml",
                ConfigureResponse = new(m => m.ConfigureHeadersAction = new HxHeaderBuilder()
                    .ReSwap("none")
                    .Build())
            });


            // temporary message model
            services.AddScoped<IComponentDescriptor>(sp => new NeedsAuthenticationComponentDescriptor<TemporaryMessageComponentLayoutModel>(() => new TemporaryMessageComponentLayoutModel
            {
                Messages = []
            })
            {
                ViewPath = "~/Components/TemporaryMessageComponentLayout.cshtml",
                ConfigureResponse = new(m => m.ConfigureHeadersAction = new HxHeaderBuilder()
                    .ReTarget("#fill-content")
                    .ReSwap("innerHTML")
                    .PushUrl("/messages")
                    .Build())
            });
            services.AddScoped<IComponentDescriptor>(_ => new NeedsAuthenticationComponentDescriptor<TemporaryMessageComponentUpdateModel>()
            {
                ViewPath = "~/Components/TemporaryMessageComponentUpdate.cshtml",
            });
            // end temporary message model

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
