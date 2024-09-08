using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;

await Host.CreateDefaultBuilder(args)
    .UseOrleans(builder => builder
        .UseLocalhostClustering()
        .Configure<ClusterOptions>(options =>
        {
            options.ClusterId = "default";
            options.ServiceId = "elysium";

        })
        .AddMemoryGrainStorage("SimpleStreamProvider")
        .AddMemoryStreams("SimpleStreamProvider"))
    .ConfigureServices(services =>
    {


    })
    .Build()
    .RunAsync();