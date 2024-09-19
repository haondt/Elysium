﻿using Elysium.Client.Services;
using Elysium.Core.Models;
using Elysium.Grains.Services;
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
using Elysium.Grains.Persistence;

namespace Elysium.Grains.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddElysiumGrainServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<HostIntegritySettings>(configuration.GetSection(nameof(HostIntegritySettings)));
            services.Configure<RemoteDocumentSettings>(configuration.GetSection(nameof(RemoteDocumentSettings)));
            services.Configure<InstanceActorSettings>(configuration.GetSection(nameof(InstanceActorSettings)));
            services.AddSingleton<IJsonLdService, JsonLdService>();
            services.AddTransient<IMemoryCache, MemoryCache>();
            //services.AddSingleton<IStoredDocumentFacade, StoredDocumentFacade>();
            services.AddSingleton<IStoredDocumentFacadeFactory, StoredDocumentFacadeFactory>();
            services.AddSingleton<IDocumentService, DocumentService>();
            services.Configure<ActivityPubHttpSettings>(configuration.GetSection(nameof(ActivityPubHttpSettings)));
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
