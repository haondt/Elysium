using Elysium.Authentication.Extensions;
using Haondt.Web.BulmaCSS.Extensions;
using Haondt.Web.Core.Extensions;
using Haondt.Web.Extensions;
using Elysium.Persistence.Extensions;
using Elysium.Hosting.Extensions;
using Elysium.Extensions;
using Haondt.Web.Core.Middleware;
using Elysium.Middlewares;
using Elysium.Client.Extensions;
using Orleans.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddApplicationPart(typeof(Haondt.Web.Extensions.ServiceCollectionExtensions).Assembly)
    .AddApplicationPart(typeof(Haondt.Web.BulmaCSS.Extensions.ServiceCollectionExtensions).Assembly)
    .AddApplicationPart(typeof(Elysium.Components.Components.HomeLayoutModel).Assembly);

builder.Configuration.AddEnvironmentVariables();

builder.Services
    .AddHaondtWebCoreServices()
    .AddHaondtWebServices(builder.Configuration)
    .UseBulmaCSS(builder.Configuration)
    .AddElysiumServices(builder.Configuration)
    .AddElysiumComponents()
    .AddElysiumAssetSources()
    .AddElysiumClientServices()
    .AddElysiumHostingServices(builder.Configuration)
    .AddElysiumPersistenceServices(builder.Configuration)
    .AddElysiumAuthenticationServices(builder.Configuration);


builder.Services.AddMvc();

builder.Services.AddOrleansClient(client => client
    .UseLocalhostClustering()
    .Configure<ClusterOptions>(options =>
    {
        options.ClusterId = "default";
        options.ServiceId = "elysium";
    }));

var app = builder.Build();

app.UseStaticFiles();
app.MapControllers();
app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseMiddleware<UnmappedRouteHandlerMiddleware>();
//app.UseAuthentication();
app.Run();

//TODO: sharedInbox
