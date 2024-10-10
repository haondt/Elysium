using Elysium.Components.Abstractions;
using Elysium.Components.Components;
using Elysium.Components.Components.Admin;
using Elysium.Components.Services;
using Haondt.Web.BulmaCSS.Extensions;
using Haondt.Web.Services;

namespace Elysium.Components.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddComponent<T>(this IServiceCollection services) where T : IComponentDescriptorFactory
        {
            return services.AddScoped(sp => ActivatorUtilities.CreateInstance<T>(sp).Create());
        }

        public static IServiceCollection AddElysiumComponentServices(this IServiceCollection services)
        {
            services.AddSingleton<ISingletonComponentFactory, SingletonComponentFactory>();

            // subset of bulma css services
            services.AddBulmaCSSHeadEntries();
            services.AddBulmaCSSAssetSources();

            services.AddTransient<ILayoutUpdateFactory, LayoutUpdateFactory>();

            return services;
        }

        public static IServiceCollection AddElysiumComponents(this IServiceCollection services)
        {
            services.AddCoreComponents();
            services.AddHomePageComponents();
            services.AddAuthenticationComponents();
            services.AddMessageComponents();
            services.AddAdminPanelComponents();
            return services;
        }

        private static IServiceCollection AddHomePageComponents(this IServiceCollection services)
        {
            services.AddComponent<ShadeSelectorComponentDescriptorFactory>();
            services.AddComponent<HomePageComponentDescriptorFactory>();
            services.AddComponent<FeedComponentDescriptorFactory>();
            services.AddComponent<MediaComponentDescriptorFactory>();

            services.AddComponent<CreatePostModalComponentDescriptorFactory>();
            return services;
        }

        private static IServiceCollection AddCoreComponents(this IServiceCollection services)
        {
            services.AddComponent<ErrorComponentDescriptorFactory>();
            services.AddComponent<CloseModalComponentDescriptorFactory>();
            services.AddComponent<DefaultLayoutComponentDescriptorFactory>();
            return services;
        }

        private static IServiceCollection AddAuthenticationComponents(this IServiceCollection services)
        {
            services.AddComponent<LoginComponentDescriptorFactory>();
            services.AddComponent<RegisterModalComponentDescriptorFactory>();
            services.AddComponent<InvitedRegisterLayoutComponentDescriptorFactory>();
            return services;
        }

        private static IServiceCollection AddAdminPanelComponents(this IServiceCollection services)
        {
            services.AddComponent<AdminPanelLayoutComponentDescriptorFactory>();
            services.AddComponent<AdminPanelLandingComponentDescriptorFactory>();
            services.AddComponent<GenerateInviteComponentDescriptorFactory>();
            return services;
        }

        private static IServiceCollection AddMessageComponents(this IServiceCollection services)
        {
            services.AddComponent<TemporaryMessageComponentDescriptorFactory>();
            services.AddComponent<TemporaryMessageUpdateComponentDescriptorFactory>();
            return services;
        }
    }
}
