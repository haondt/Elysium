using Elysium.ActivityPub.Helpers;
using Elysium.ActivityPub.Models;
using Elysium.Authentication.Services;
using Elysium.Client.Hubs;
using Elysium.Components.Components;
using Elysium.Core.Extensions;
using Elysium.Core.Models;
using Elysium.GrainInterfaces.Client;
using Haondt.Identity.StorageKey;
using Haondt.Web.Core.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Infrastructure;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace Elysium.Client.Services
{
    public class ClientActorActivityDeliveryObserver(
        string connectionId,
        StorageKey<UserIdentity> identity,
        IServiceScopeFactory scopeFactory,
        IHubContext<ElysiumHub> hub
        ) : IClientActorActivityDeliveryObserver
    {
        private int _counter = 0;
        public async Task ReceiveActivity(ClientIncomingActivityDetails details)
        {
            var type = ActivityPubJsonNavigator.GetType(details.ExpandedObject);
            if (type != JsonLdTypes.NOTE)
                // todo: decide how to handle this
                throw new NotImplementedException();

            //await hub.Clients.Client(connectionId).SendAsync("ReceiveMessage", CreateView(details));
            var result = await CreateView(details);
            await hub.Clients.Client(connectionId).SendAsync("ReceiveMessage", result);
            _counter++;
        }

        public async Task<string> CreateView(ClientIncomingActivityDetails details)
        {
            var messageText = ActivityPubJsonNavigator.GetValue(details.ExpandedObject, JsonLdTypes.CONTENT).AsString();
            var timestamp = (DateTime)ActivityPubJsonNavigator.GetValue(details.ExpandedObject, JsonLdTypes.PUBLISHED);

            using var scope = scopeFactory.CreateScope();
            var componentFactory = scope.ServiceProvider.GetRequiredService<IComponentFactory>();
            var sessionService = scope.ServiceProvider.GetRequiredService<ISessionService>();
            var elysiumService = scope.ServiceProvider.GetRequiredService<IElysiumService>();
            if (sessionService is ProxySessionService proxySessionService)
                proxySessionService.SessionService = new StaticSessionService(identity);

            string author = details.SenderPreferredUsername.HasValue
                ? elysiumService.GetFediverseUsername(details.SenderPreferredUsername.Value, details.Sender.Host)
                : details.Sender.ToString();

            var updaterComponent = await componentFactory.GetPlainComponent(new TemporaryMessageComponentUpdateModel
            {
                AddMessages =
                [
                    new() {
                            Author = author,
                            Text = messageText,
                            TimeStamp = timestamp
                        }
                ]
            });

            // in a controller I would use this extension method
            //public static ViewResult CreateView<T>(this IComponent<T> component, Controller controller) where T : IComponentModel
            //{
            //    if (component.ConfigureResponse.HasValue)
            //    {
            //        HttpResponseMutator httpResponseMutator = new HttpResponseMutator();
            //        component.ConfigureResponse.Value(httpResponseMutator);
            //        httpResponseMutator.Apply(controller.Response.AsResponseData());
            //    }

            //    return controller.View(component.ViewPath, component.Model);
            //}
            // and call it like
            // updatedComponent.CreateView(this);

            // but in this context I am just a callback from another process. How can I render this component?

            var actionContext = new ActionContext
            {
                HttpContext = new DefaultHttpContext
                {
                    RequestServices = scope.ServiceProvider
                },
                RouteData = new Microsoft.AspNetCore.Routing.RouteData(),
                ActionDescriptor = new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()
            };

            var tempDataProvider = actionContext.HttpContext.RequestServices.GetRequiredService<TempDataSerializer>();

            using var writer = new StringWriter();
            var viewEngine = scope.ServiceProvider.GetRequiredService<IRazorViewEngine>();
            var viewResult = viewEngine.GetView(null, updaterComponent.ViewPath, false);
            if (viewResult == null)
                throw new FileNotFoundException($"View '{updaterComponent.ViewPath}' not found.");


            var viewContext = new ViewContext(
                actionContext,
                viewResult.View,
                new ViewDataDictionary<object>(
                    new EmptyModelMetadataProvider(),
                    new ModelStateDictionary())
                {
                    Model = updaterComponent.Model
                },
                new TempDataDictionary(
                    actionContext.HttpContext,
                    new SessionStateTempDataProvider(tempDataProvider)),
                writer,
                new HtmlHelperOptions());

            await viewResult.View.RenderAsync(viewContext);
            return writer.ToString();

            //await using var renderer = new HtmlRenderer(scope.ServiceProvider, scope.ServiceProvider.GetRequiredService<ILoggerFactory>());

            //var html = await renderer.Dispatcher.InvokeAsync(async () =>
            //{
            //    var parameters = ParameterView.FromDictionary(new Dictionary<string, object?>());
            //    var output = await renderer.RenderComponentAsync()
            //    return output.ToString();   
            //})


            throw new NotImplementedException();


        }
    }
}
