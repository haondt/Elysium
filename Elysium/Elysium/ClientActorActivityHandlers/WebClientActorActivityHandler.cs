using Elysium.ActivityPub.Helpers;
using Elysium.ActivityPub.Models;
using Elysium.Client.Hubs;
using Elysium.Client.Services;
using Elysium.Components.Components;
using Elysium.Core.Extensions;
using Elysium.GrainInterfaces.Client;
using Haondt.Core.Models;
using Haondt.Web.Core.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Infrastructure;

namespace Elysium.ClientActorActivityHandlers
{
    public class WebClientActorActivityHandler(
        IComponentFactory componentFactory,
        IServiceProvider serviceProvider,
        IRazorViewEngine razorViewEngine,
        IElysiumService elysiumService) : IClientActorActivityHandler
    {
        public string ClientType => "web";

        public async Task<Optional<(string Method, Optional<object> Arg)>> HandleAsync(ClientIncomingActivityDetails details)
        {
            var type = ActivityPubJsonNavigator.GetType(details.ExpandedObject);
            if (type != JsonLdTypes.NOTE)
                // todo: decide how to handle this
                throw new NotImplementedException();
            var view = await CreateView(details);

            return new(("ReceiveMessage", new(view)));
        }

        public async Task<string> CreateView(ClientIncomingActivityDetails details)
        {
            var messageText = ActivityPubJsonNavigator.GetValue(details.ExpandedObject, JsonLdTypes.CONTENT).AsString();
            var timestamp = (DateTime)ActivityPubJsonNavigator.GetValue(details.ExpandedObject, JsonLdTypes.PUBLISHED);

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


            var actionContext = new ActionContext
            {
                HttpContext = new DefaultHttpContext
                {
                    RequestServices = serviceProvider
                },
                RouteData = new Microsoft.AspNetCore.Routing.RouteData(),
                ActionDescriptor = new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()
            };

            var tempDataProvider = actionContext.HttpContext.RequestServices.GetRequiredService<TempDataSerializer>();

            using var writer = new StringWriter();
            var viewResult = razorViewEngine.GetView(null, updaterComponent.ViewPath, false);
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

        }
    }
}
