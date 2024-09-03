using Elysium.Authentication.Extensions;
using Elysium.Extensions;
using Haondt.Identity.StorageKey;
using Haondt.Web.BulmaCSS.Extensions;
using Haondt.Web.Core.Extensions;
using Haondt.Web.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using Elysium.Persistence.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddApplicationPart(typeof(Haondt.Web.Extensions.ServiceCollectionExtensions).Assembly)
    .AddApplicationPart(typeof(Haondt.Web.BulmaCSS.Extensions.ServiceCollectionExtensions).Assembly);

builder.Configuration.AddEnvironmentVariables();

builder.Services
    .AddHaondtWebCoreServices()
    .AddHaondtWebServices(builder.Configuration)
    .UseBulmaCSS(builder.Configuration)
    .AddElysiumServices()
    .AddElysiumPersistenceServices(builder.Configuration)
    .AddElysiumAuthenticationServices(builder.Configuration);


builder.Services.AddMvc();

var app = builder.Build();

app.UseStaticFiles();
app.MapControllers();
//app.UseAuthentication();
app.Run();
