using Elysium.Authentication.Extensions;
using Elysium.Client.Extensions;
using Elysium.Client.Hubs;
using Elysium.Components.Extensions;
using Elysium.Core.Extensions;
using Elysium.Domain.Extensions;
using Elysium.Extensions;
using Elysium.Hosting.Extensions;
using Elysium.Middlewares;
using Elysium.Persistence.Extensions;
using Haondt.Web.Core.Extensions;
using Haondt.Web.Core.Middleware;
using Haondt.Web.Extensions;

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
    .ConfigureStorageKeyConvert()
    .AddElysiumAssetSources()
    .AddElysiumClientServices()
    .AddElysiumDomainServices(builder.Configuration)
    .AddElysiumHostingServices(builder.Configuration)
    .AddElysiumPersistenceServices(builder.Configuration)
    .AddElysiumAuthenticationServices(builder.Configuration);


builder.Services.AddMvc();

builder.Services.AddSignalR();

builder.Services.AddOrleansClient(client => client
    .ConfigureCluster(builder.Configuration));

var app = builder.Build();

app.UseStaticFiles();
app.MapControllers();
app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseMiddleware<UnmappedRouteHandlerMiddleware>();
app.MapHub<ElysiumHub>("/elysiumHub");
//app.UseAuthentication();
app.Run();

//TODO: sharedInbox
