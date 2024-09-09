using Elysium.Silo.Extensions;
using Elysium.Persistence.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;
using Elysium.GrainInterfaces;
using Elysium.Hosting.Extensions;
using Elysium.Grains.Extensions;
using Elysium.Authentication.Extensions;

await Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

    })
    .UseOrleans(builder => builder
        .UseLocalhostClustering()
        .Configure<ClusterOptions>(options =>
        {
            options.ClusterId = "default";
            options.ServiceId = "elysium";

        })
        .AddMemoryGrainStorageAsDefault()
        .AddMemoryGrainStorage(GrainConstants.SimpleStreamProvider)
        .AddMemoryGrainStorage(GrainConstants.GrainDocumentStorage)
        .AddMemoryGrainStorage(GrainConstants.GrainStorage)
        .AddMemoryStreams(GrainConstants.SimpleStreamProvider))
    .ConfigureServices((context, services) =>
    {
        services
            .AddElysiumSiloServices()
            .AddElysiumGrainServices(context.Configuration)
            .AddElysiumCryptoServices()
            .AddElysiumHostingServices(context.Configuration)
            .AddElysiumPersistenceServices(context.Configuration);
    })
    .Build()
    .RunAsync();