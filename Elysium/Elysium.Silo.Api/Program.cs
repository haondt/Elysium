using Elysium.GrainInterfaces;
using Elysium.Grains.Extensions;
using Elysium.Silo.Extensions;
using Orleans.Configuration;
using Elysium.Hosting.Extensions;
using Elysium.Persistence.Extensions;
using Elysium.Cryptography.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddControllers()
    .AddNewtonsoftJson(options =>
    {
    })
    .AddApplicationPart(typeof(Elysium.ActivityPub.Api.Controllers.ActorController).Assembly);

builder.Configuration.AddEnvironmentVariables();

//builder.Services

builder.Host
    .UseOrleans(builder => builder
        .UseLocalhostClustering()
        .Configure<ClusterOptions>(options =>
        {
            options.ClusterId = "default";
            options.ServiceId = "elysium";
        })
        .AddElysiumStorageGrainStorage(GrainConstants.SimpleStreamProvider)
        .AddElysiumStorageGrainStorage(GrainConstants.GrainDocumentStorage)
        .AddElysiumStorageGrainStorage(GrainConstants.GrainStorage)
        .AddMemoryStreams(GrainConstants.SimpleStreamProvider))
    .ConfigureServices((context, services) =>
    {
        services
            .AddElysiumSiloServices()
            .AddElysiumGrainServices(context.Configuration)
            .AddElysiumCryptoServices()
            .AddElysiumHostingServices(context.Configuration)
            .AddElysiumPersistenceServices(context.Configuration);
    });

var app = builder.Build();

// Configure the HTTP request pipeline.

//app.UseAuthorization();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.MapControllers();
app.Run();
