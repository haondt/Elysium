using DotNext;
using Elysium.Components;
using Elysium.Components.Components;
using Haondt.Web.Core.Components;
using Haondt.Web.Core.Services;

namespace Elysium.Components.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddElysiumComponents(this IServiceCollection services)
        {
            services.AddSingleton<IComponentDescriptor>(sp => new ComponentDescriptor<HomeLayoutModel>(async (cf, rd) =>
            {
                //var session = sp.GetRequiredService<ISession>
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
            services.AddSingleton<IComponentDescriptor>(new ComponentDescriptor<FeedModel>()
            {
                ViewPath = "~/Components/Feed.cshtml",
            });
            services.AddSingleton<IComponentDescriptor>(new ComponentDescriptor<ErrorModel>()
            {
                ViewPath = "~/Components/Error.cshtml",
                ConfigureResponse = new(m => m.ConfigureHeadersAction = new HxHeaderBuilder()
                    .ReTarget("#content")
                    .ReSwap("innerHTML")
                    .Build())
            });
            services.AddSingleton<IComponentDescriptor>(new ComponentDescriptor<LoginModel>()
            {
                ViewPath = "~/Components/Login.cshtml",
                ConfigureResponse = new(m => m.ConfigureHeadersAction = new HxHeaderBuilder()
                    .ReTarget("#content")
                    .ReSwap("innerHTML")
                    .PushUrl("login")
                    .Build())
            });
            return services;
        }
    }
}
