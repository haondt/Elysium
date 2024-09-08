using Elysium.Core.Models;
using Elysium.GrainInterfaces.Services;
using Elysium.Grains.Services;
using Elysium.Hosting.Models;
using Haondt.Identity.StorageKey;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddElysiumGrainServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<HostIntegritySettings>(configuration.GetSection(nameof(HostIntegritySettings)));
            services.Configure<RemoteDocumentSettings>(configuration.GetSection(nameof(RemoteDocumentSettings)));
            services.AddScoped<ITypedActorServiceProvider, TypedActorServiceFactory>();
            services.AddSingleton<IGrainFactory<LocalUri>, LocalUriGrainFactory>();
            services.AddSingleton<IGrainFactory<RemoteUri>, RemoteUriGrainFactory>();
            //services.AddSingleton<IGrainFactory<StorageKey<UserIdentity>>, StorageKeyGrainFactory<UserIdentity>>();
            services.AddSingleton<IGrainFactory<StorageKey>, StorageKeyGrainFactory>();
            services.AddScoped<IDocumentResolver, DocumentResolver>();
            services.AddHttpClient<IActivityPubHttpService, ActivityPubHttpService>(client =>
            {
                client.DefaultRequestHeaders.Add("Accept", "application/ld+json; profile=\"https://www.w3.org/ns/activitystreams\"");
            }).AddPolicyHandler(HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrTransientHttpStatusCode()
                .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt))));
            return services;
        }
    }
}
