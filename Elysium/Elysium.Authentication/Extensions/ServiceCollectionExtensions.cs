using Elysium.Authentication.Components;
using Elysium.Authentication.Constants;
using Elysium.Authentication.Services;
using Elysium.Core.Models;
using Elysium.Cryptography.Extensions;
using Haondt.Persistence.Extensions;
using Haondt.Web.Core.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Elysium.Authentication.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddElysiumAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHaondtPersistenceServices(configuration);
            services.AddIdentity<UserIdentity, RoleIdentity>()
                .AddUserStore<ElysiumUserStore>()
                .AddRoleStore<ElysiumRoleStore>();
            services.Configure<IdentityOptions>(o =>
            {
                o.User.AllowedUserNameCharacters = AuthenticationConstants.ALLOWED_USERNAME_CHARACTERS;
            });
            services.AddScoped<SessionService>();
            services.AddScoped<ISessionService, ProxySessionService>(sp =>
                ActivatorUtilities.CreateInstance<ProxySessionService>(
                    sp, sp.GetRequiredService<SessionService>()));
            services.AddScoped<ComponentFactory>();
            services.AddScoped<IComponentFactory>(sp =>
                ActivatorUtilities.CreateInstance<VerifiesAuthorizationComponentFactory>(
                    sp, sp.GetRequiredService<ComponentFactory>()));
            services.AddElysiumCryptoServices(configuration);

            return services;
        }

    }
}
