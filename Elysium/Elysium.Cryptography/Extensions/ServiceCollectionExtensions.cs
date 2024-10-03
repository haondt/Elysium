using Elysium.Cryptography.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
