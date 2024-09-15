using Elysium.Authentication.Services;
using Elysium.Cryptography.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Elysium.Cryptography.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddElysiumCryptoServices(this IServiceCollection services)
        {
            services.AddDataProtection(p => p.ApplicationDiscriminator = "Elysium");
            services.AddSingleton<ICryptoService, CryptoService>();
            services.AddSingleton<IUserCryptoService, UserCryptoService>();
            return services;
        }
    }
}
