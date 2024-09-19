using Elysium.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddElysiumStorageKeyConverter(this IServiceCollection services)
        {
            services.AddSingleton<IElysiumStorageKeyConverter, ElysiumStorageKeyConverter>();
            return services;
        }
    }
}
