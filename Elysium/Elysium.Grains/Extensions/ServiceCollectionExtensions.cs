using Elysium.Grains.Services;
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
            services.Configure<HostingSettings>(configuration.GetSection(nameof(HostingSettings)));
            services.Configure<HostIntegritySettings>(configuration.GetSection(nameof(HostIntegritySettings)));
            services.Configure<RemoteDocumentSettings>(configuration.GetSection(nameof(RemoteDocumentSettings)));
            services.AddScoped<IActivityPubService, ActivityPubService>();
            services.AddScoped<IDocumentResolver, DocumentResolver>();
            services.AddScoped<IActivityPubJsonNavigator, ActivityPubJsonNavigator>();
            services.AddScoped<IHostingService, HostingService>();
            services.AddHttpClient<IGrainHttpClient<RemoteDocumentGrain>, GrainHttpClient<RemoteDocumentGrain>>(client =>
            {
                client.DefaultRequestHeaders.Add("Accept", "application/ld+json; profile=\"https://www.w3.org/ns/activitystreams\"");
            }).AddPolicyHandler(HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrTransientHttpStatusCode()
                .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt))));
            services.AddHttpClient<IGrainHttpClient<DispatchRemoteActivityWorkerGrain>, GrainHttpClient<DispatchRemoteActivityWorkerGrain>>(client =>
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
