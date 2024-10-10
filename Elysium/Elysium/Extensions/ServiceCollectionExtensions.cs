using Elysium.Client.Hubs;
using Elysium.Client.Services;
using Elysium.ClientActorActivityHandlers;
using Elysium.ClientStartupParticipants;
using Elysium.EventHandlers;
using Elysium.EventHandlers.Authentication;
using Elysium.EventHandlers.PublishActivity;
using Elysium.Middlewares;
using Elysium.Middlewares.Services;
using Elysium.Services;
using Haondt.Web.Assets;
using Haondt.Web.Core.Services;
using Haondt.Web.Services;

namespace Elysium.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddElysiumServices(this IServiceCollection services, IConfiguration configuration)
        {
            // middleware services
            services.Configure<ErrorSettings>(configuration.GetSection(nameof(ErrorSettings)));
            services.AddSingleton<IExceptionActionResultFactory, ElysiumExceptionActionResultFactory>();

            // component factories
            services.AddSingleton<ISingletonPageComponentFactory, SingletonPageComponentFactory>();
            services.AddScoped<IComponentHandler, ElysiumComponentHandler>();

            // startup participants
            services.Configure<AdminSettings>(configuration.GetSection(nameof(AdminSettings)));
            services.AddScoped<IClientStartupParticipant, RoleRegisterer>();
            services.AddScoped<IClientStartupParticipant, DefaultAdminAccountRegisterer>();

            // signalr handlers 
            services.AddScoped<IClientActorActivityHandler, WebClientActorActivityHandler>();

            // event handlers
            services.AddScoped<IEventHandler, SingleEventHandlerRegistry>();
            services.AddAuthenticationEventHandlers(configuration);
            services.AddPublishActivityEventHandlers();

            return services;
        }

        public static IServiceCollection AddElysiumHeadEntries(this IServiceCollection services)
        {
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
            services.AddScoped<IHeadEntryDescriptor>(_ => new ScriptDescriptor
            {
                Uri = "https://unpkg.com/@microsoft/signalr@8.0.7/dist/browser/signalr.js"
            });
            services.AddScoped<IHeadEntryDescriptor>(_ => new ScriptDescriptor
            {
                Uri = $"/_asset/{assemblyPrefix}.wwwroot.hx-signalr.js"
            });
            services.AddScoped<IHeadEntryDescriptor>(_ => new ScriptDescriptor
            {
                Uri = "https://unpkg.com/htmx.org@1.9.12/dist/ext/ws.js"
            });
            return services;
        }

        private static IServiceCollection AddAuthenticationEventHandlers(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ISingleEventHandler, LoginUserEventHandler>();
            services.AddScoped<InviteStateAgnosticRegisterUserEventHandler>();
            services.Configure<RegistrationSettings>(configuration.GetSection(nameof(RegistrationSettings)));
            services.AddScoped<ISingleEventHandler, RegisterUserEventHandler>();
            services.AddScoped<ISingleEventHandler, InvitedRegisterUserEventHandler>();
            services.AddScoped<ISingleEventHandler, GenerateInviteLinkEventHandler>();
            return services;
        }

        private static IServiceCollection AddPublishActivityEventHandlers(this IServiceCollection services)
        {
            services.AddScoped<ISingleEventHandler, CreatePostEventHandler>();
            services.AddScoped<ISingleEventHandler, SendMessageEventHandler>();
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
