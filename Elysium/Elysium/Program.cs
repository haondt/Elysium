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
using Elysium.Client.Hubs;
using Elysium.Components.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddApplicationPart(typeof(Haondt.Web.Extensions.ServiceCollectionExtensions).Assembly)
    .AddApplicationPart(typeof(Haondt.Web.BulmaCSS.Extensions.ServiceCollectionExtensions).Assembly)
    .AddApplicationPart(typeof(Elysium.Components.Components.HomePageModel).Assembly);

builder.Configuration.AddEnvironmentVariables();

builder.Services
    .AddHaondtWebCoreServices()
    .AddHaondtWebServices(builder.Configuration)
    .AddElysiumComponentServices() // must come before AddElysiumServices so bulma css overrides get loaded in correct order
    .AddElysiumServices(builder.Configuration)
    .AddElysiumComponents()
    .AddElysiumAssetSources()
    .AddElysiumClientServices()
    .AddElysiumHostingServices(builder.Configuration)
    .AddElysiumPersistenceServices(builder.Configuration)
    .AddElysiumAuthenticationServices(builder.Configuration);


builder.Services.AddMvc();

builder.Services.AddSignalR();

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
app.MapHub<ElysiumHub>("/elysiumHub");
//app.UseAuthentication();
app.Run();

//TODO: sharedInbox
