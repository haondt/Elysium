using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

await Host.CreateDefaultBuilder(args)
    .UseOrleans(builder => builder
        .UseLocalhostClustering()
        .AddMemoryGrainStorage("Elysium")
        .AddMemoryStreams("SimpleStreamProvider"))
    .ConfigureServices(services =>
    {


    })
    .Build()
    .RunAsync();