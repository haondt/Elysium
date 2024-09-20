using Elysium.Client.Services;
using Elysium.Core.Models;
using Elysium.Domain.Services;
using Elysium.Core.Extensions;

using Haondt.Identity.StorageKey;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elysium.GrainInterfaces;

namespace Elysium.Domain.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddElysiumGrainServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<HostIntegritySettings>(configuration.GetSection(nameof(HostIntegritySettings)));
            services.Configure<InstanceActorSettings>(configuration.GetSection(nameof(InstanceActorSettings)));
            services.AddTransient<IMemoryCache, MemoryCache>();

            return services;
        }
    }
}
