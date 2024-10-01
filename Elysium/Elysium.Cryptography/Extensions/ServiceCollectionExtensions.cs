using Elysium.Core.Extensions;
using Elysium.Core.Models;
using Elysium.Cryptography.Services;
using Haondt.Core.Extensions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Elysium.Cryptography.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddElysiumCryptoServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ICryptoService, CryptoService>();
            services.AddSingleton<IUserCryptoService, UserCryptoService>();
            services.Configure<UserCryptoSettings>(configuration.GetSection(nameof(UserCryptoSettings)));
            return services;
        }
    }
}
