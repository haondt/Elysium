using Elysium.Authentication.Services;
using Elysium.Core.Models;
using Haondt.Identity.StorageKey;
using Haondt.Persistence.Extensions;
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
            services.AddScoped<IEventHandler, AuthenticationEventHandler>();
            services.AddScoped<ISessionService, SessionService>();
            services.AddOptions<CryptoSettings>()
                .Bind(configuration.GetSection(nameof(CryptoSettings)))
                .ValidateOnStart();
            services.AddSingleton<IValidateOptions<CryptoSettings>, CryptoSettingsValidator>();
            services.AddScoped<ICryptoService, CryptoService>();
            services.AddScoped<IUserCryptoService, UserCryptoService>();
            return services;
        }
    }
}
