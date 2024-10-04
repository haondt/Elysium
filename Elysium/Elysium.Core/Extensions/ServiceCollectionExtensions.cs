using Haondt.Identity.StorageKey;
using Microsoft.Extensions.DependencyInjection;

namespace Elysium.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureStorageKeyConvert(this IServiceCollection services)
        {
            StorageKeyConvert.DefaultSerializerSettings = new StorageKeySerializerSettings
            {
                TypeNameStrategy = TypeNameStrategy.SimpleTypeConverter,
                KeyEncodingStrategy = KeyEncodingStrategy.String
            };
            return services;
        }
    }
}
