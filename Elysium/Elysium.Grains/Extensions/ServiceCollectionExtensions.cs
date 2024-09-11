using Elysium.Core.Models;
using Elysium.GrainInterfaces.Services;
using Elysium.Grains.Services;
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
            services.AddSingleton<IJsonLdService, JsonLdService>();
            //services.AddSingleton<IStoredDocumentFacade, StoredDocumentFacade>();
            services.AddSingleton<IStoredDocumentFacadeFactory, StoredDocumentFacadeFactory>();
            services.AddSingleton<IDocumentService, DocumentService>();
            services.AddHttpClient<IActivityPubHttpService, ActivityPubHttpService>(client =>
            {
                client.DefaultRequestHeaders.Add("Accept", "application/ld+json; profile=\"https://www.w3.org/ns/activitystreams\"");
            }).AddPolicyHandler(HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrTransientHttpStatusCode()
                // todo: appsettings that shii
                .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt))));
            return services;
        }
    }
}
