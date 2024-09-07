using DotNext;
using Elysium.Authentication.Services;
using Elysium.Components.Components;
using Elysium.Services;
using Haondt.Web.Assets;
using Haondt.Web.Core.Components;
using Haondt.Web.Core.Http;
using Haondt.Web.Core.Services;
using Haondt.Web.Services;
using Haondt.Web.Core.Extensions;
using Haondt.Web.Components;

namespace Elysium.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddElysiumServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ErrorSettings>(configuration.GetSection(nameof(ErrorSettings)));
            services.AddScoped<IEventHandler, ElysiumPublishActivityEventHandler>(); 
            services.AddSingleton<ISingletonComponentFactory, SingletonComponentFactory>();
            services.AddSingleton<ISingletonPageComponentFactory, SingletonPageComponentFactory>();
            services.AddSingleton<IExceptionActionResultFactory, ElysiumExceptionActionResultFactory>();
            services.AddScoped<IComponentHandler, ElysiumComponentHandler>();

            var assemblyPrefix = typeof(ServiceCollectionExtensions).Assembly.GetName().Name;
            services.AddScoped<IHeadEntryDescriptor>(sp => new IconDescriptor
            {
                Uri = $"/_asset/{assemblyPrefix}.wwwroot.icon.ico"
            });
            services.AddScoped<IHeadEntryDescriptor>(sp => new StyleSheetDescriptor
            {
                Uri = $"/_asset/{assemblyPrefix}.wwwroot.bulma-custom.css"
            });
            services.AddScoped<IHeadEntryDescriptor>(_ => new MetaDescriptor
            {
                Name = "htmx-config",
                Content = @"{
                    ""responseHandling"": [
                        { ""code"": ""204"", ""swap"": false },
                        { ""code"": "".*"", ""swap"": true }
                    ]
                }",
            });
            return services;
        }

        public static IServiceCollection AddElysiumComponents(this IServiceCollection services)
        {
            services.AddScoped<IComponentDescriptor>(sp => new ComponentDescriptor<HomeLayoutModel>(async (cf, rd) =>
            {
                var session = sp.GetRequiredService<ISessionService>();
                if (!session.IsAuthenticated())
                    return new Result<HomeLayoutModel>(new UnauthorizedAccessException());

                var feed = await cf.GetComponent(new FeedModel());
                if (!feed.IsSuccessful)
                    return new Result<HomeLayoutModel>(feed.Error);
                return new(new HomeLayoutModel
                {
                    Feed = feed.Value
                });
            })
            {
                ViewPath = "~/Components/HomeLayout.cshtml",
            });
            services.AddScoped<IComponentDescriptor>(_ => new ComponentDescriptor<FeedModel>()
            {
                ViewPath = "~/Components/Feed.cshtml",
            });
            services.AddScoped<IComponentDescriptor>(_ => new ComponentDescriptor<ErrorModel>((componentFactory, requestData) =>
            {
                var errorCodeResult = requestData.Query.GetValue<int>("errorCode");
                var messageResult = requestData.Query.GetValue<string>("message");
                if (!errorCodeResult.IsSuccessful || !messageResult.IsSuccessful)
                    return new(new InvalidOperationException("Not enough information to construct ErrorModel"));
                var titleResult = requestData.Query.GetValue<string>("title");
                var detailsResult = requestData.Query.GetValue<string>("details");
                return new(new ErrorModel
                {
                    ErrorCode = errorCodeResult.Value,
                    Message = messageResult.Value,
                    Title = titleResult,
                    Details = detailsResult,
                });
            })
            {
                ViewPath = "~/Components/Error.cshtml",
                ConfigureResponse = new(m => m.ConfigureHeadersAction = new HxHeaderBuilder()
                    .ReTarget("#content")
                    .ReSwap("innerHTML")
                    .Build())
            });
            services.AddScoped<IComponentDescriptor>(_ => new ComponentDescriptor<LoginModel>(new LoginModel())
            {
                ViewPath = "~/Components/Login.cshtml",
                ConfigureResponse = new(m => m.ConfigureHeadersAction = new HxHeaderBuilder()
                    .ReTarget("#content")
                    .ReSwap("innerHTML")
                    .PushUrl("/identity/login")
                    .Build())
            });
            services.AddScoped<IComponentDescriptor>(_ => new ComponentDescriptor<CloseModalModel>(new CloseModalModel())
            {
                ViewPath = "~/Components/CloseModal.cshtml"
            });
            services.AddScoped<IComponentDescriptor>(_ => new ComponentDescriptor<RegisterModalModel>(new RegisterModalModel())
            {
                ViewPath = "~/Components/RegisterModal.cshtml",
                ConfigureResponse = new(m => m.ConfigureHeadersAction = new HxHeaderBuilder()
                    .ReSwap("none")
                    .Build())
            });


            // temporary message model
            services.AddScoped<IComponentDescriptor>(sp => new ComponentDescriptor<TemporaryMessageComponentLayoutModel>(async (cf, rd) =>
            {
                var session = sp.GetRequiredService<ISessionService>();
                if (!session.IsAuthenticated())
                    return new Result<TemporaryMessageComponentLayoutModel>(new UnauthorizedAccessException());

                //var loaderComponent = await cf.GetPlainComponent(new LoaderModel
                //{
                //    Target = $"/_component/{ComponentDescriptor<TemporaryMessageUpdateModel>.TypeIdentity}"
                //});
                //if (!loaderComponent.IsSuccessful)
                //    return new(loaderComponent.Error);

                //var appendComponent = await cf.GetPlainComponent(new AppendComponentLayoutModel
                //{
                //    Components = [loaderComponent.Value, .Value]
                //}, configureResponse: m =>
                //{
                //    m.ConfigureHeadersAction = new HxHeaderBuilder()
                //        .ReSwap("innerHTML")
                //        .ReTarget("#content")
                //        .PushUrl("/home")
                //        .Build();
                //});



                //var feed = await cf.GetComponent(new FeedModel());
                //if (!feed.IsSuccessful)
                //    return new Result<FeedModel>(feed.Error);
                return new(new TemporaryMessageComponentLayoutModel
                {
                    Messages = []
                });
            })
            {
                ViewPath = "~/Components/TemporaryMessageComponentLayout.cshtml",
                ConfigureResponse = new(m => m.ConfigureHeadersAction = new HxHeaderBuilder()
                    .ReTarget("#fill-content")
                    .ReSwap("innerHTML")
                    .PushUrl("/messages")
                    .Build())
            });
            services.AddScoped<IComponentDescriptor>(_ => new ComponentDescriptor<TemporaryMessageComponentUpdateModel>()
            {
                ViewPath = "~/Components/TemporaryMessageComponentUpdate.cshtml",
            });

            // end temporary message model

            return services;
        }

        public static IServiceCollection AddElysiumAssetSources(this IServiceCollection services)
        {
            var assembly = typeof(ServiceCollectionExtensions).Assembly;
            services.AddSingleton<IAssetSource>(sp => new ManifestAssetSource(assembly));
            return services;
        }
    }
}
