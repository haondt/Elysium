using Elysium.Authentication.Services;
using Elysium.Core.Models;
using Haondt.Identity.StorageKey;
using Haondt.Persistence.Extensions;
using Haondt.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            return services;
        }
    }
}
