using Elysium.Authentication.Components;
using Elysium.Authentication.Constants;
using Elysium.Authentication.Services;
using Elysium.Core.Models;
using Haondt.Identity.StorageKey;
using Haondt.Persistence.Extensions;
using Haondt.Web.Core.Components;
using Haondt.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                ActivatorUtilities.CreateInstance<VerifiesAuthenticationComponentFactory>(
                    sp, sp.GetRequiredService<ComponentFactory>()));
            services.AddElysiumCryptoServices();
            return services;
        }

        public static IServiceCollection AddElysiumCryptoServices(this IServiceCollection services)
        {
            services.AddDataProtection(p => p.ApplicationDiscriminator = "Elysium");
            services.AddSingleton<ICryptoService, CryptoService>();
            services.AddSingleton<IUserCryptoService, UserCryptoService>();
            return services;
        }
    }
}
