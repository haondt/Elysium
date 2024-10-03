using Elysium.Core.Extensions;
using Elysium.Cryptography.Extensions;
using Elysium.Domain.Extensions;
using Elysium.GrainInterfaces;
using Elysium.Hosting.Extensions;
using Elysium.Persistence.Extensions;
using Elysium.Silo.Extensions;

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
    .UseOrleans((context, builder) => builder
        .ConfigureCluster(context.Configuration)
        .AddElysiumStorageGrainStorage(GrainConstants.SimpleStreamProvider)
        .AddElysiumStorageGrainStorage(GrainConstants.GrainDocumentStorage)
        .AddElysiumStorageGrainStorage(GrainConstants.GrainStorage)
        .AddMemoryStreams(GrainConstants.SimpleStreamProvider))
    .ConfigureServices((context, services) =>
    {
        services
            .ConfigureStorageKeyConvert()
            .AddElysiumSiloServices()
            .AddElysiumGrainServices(context.Configuration)
            .AddElysiumDomainServices(context.Configuration)
            .AddElysiumCryptoServices(context.Configuration)
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
