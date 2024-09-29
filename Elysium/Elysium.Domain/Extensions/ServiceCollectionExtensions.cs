using Elysium.Client.Services;
using Elysium.Core.Models;
using Elysium.Domain.Services;
using Elysium.Core.Extensions;

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
using Elysium.GrainInterfaces;
using Elysium.GrainInterfaces.Services;

namespace Elysium.Domain.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddElysiumDomainServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IJsonLdService, JsonLdService>();
            services.AddSingleton<IStoredDocumentFacadeFactory, StoredDocumentFacadeFactory>();
            services.AddSingleton<IDocumentService, DocumentService>();
            services.Configure<ActivityPubHttpSettings>(configuration.GetSection(nameof(ActivityPubHttpSettings)));
            services.AddHttpClient<IActivityPubHttpService, ActivityPubHttpService>(client =>
            {
                client.DefaultRequestHeaders.Add("Accept", "application/ld+json; profile=\"https://www.w3.org/ns/activitystreams\"");
            }).ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler
                {
                    AllowAutoRedirect = false
                };
            }).AddPolicyHandler(HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrTransientHttpStatusCode()
                // todo: appsettings that shii
                .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt))));
            services.AddElysiumDomainGrainFactories(configuration);

            return services;
        }

        public static IServiceCollection AddElysiumDomainGrainFactories(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IGrainFactory<Iri>, IriGrainFactory>();
            return services;
        }
    }
}
