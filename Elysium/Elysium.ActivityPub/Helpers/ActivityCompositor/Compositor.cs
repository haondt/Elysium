using Elysium.ActivityPub.Models;
using Elysium.Core.Extensions;
using Newtonsoft.Json.Linq;

namespace Elysium.ActivityPub.Helpers.ActivityCompositor
{
    public static class Compositor
    {
        public static JArray Composit(ICompositionDetails details)
        {
            switch (details)
            {
                case MessageDetails messageDetails:
                    return new ActivityPubJsonBuilder()
                        .Type(messageDetails.Type)
                        .To(messageDetails.Recepient)
                        .AttributedTo(messageDetails.AttributedTo)
                        .Published(DateTime.UtcNow)
                        .Content(messageDetails.Text)
                        .Build();
                case CreateActivityDetails activityDetails:
                    return new ActivityPubJsonBuilder()
                        .Id(activityDetails.Id)
                        .Type(activityDetails.Type)
                        .Actor(activityDetails.Actor)
                        .Cc(activityDetails.Cc)
                        .To(activityDetails.To)
                        .Bto(activityDetails.Bto)
                        .Bcc(activityDetails.Bcc)
                        .AttributedTo(activityDetails.AttributedTo)
                        .Object(activityDetails.Object)
                        .Build();
            }

            return new(new InvalidOperationException($"Unkown object details type {details.GetType()}"));
        }

        public static JArray Composit(PrePublishActivityDetails details)
        {
            var activityClone = details.ReferencedActivityWithBtoBcc.DeepClone();
            var activityMainObjectCloneResult = activityClone
                .Single()
                .As<JObject>();

            // strip bcc, bto from activity
            if (activityMainObjectCloneResult.ContainsKey(JsonLdTypes.BCC))
                activityMainObjectCloneResult.Remove(JsonLdTypes.BCC);
            if (activityMainObjectCloneResult.ContainsKey(JsonLdTypes.BTO))
                activityMainObjectCloneResult.Remove(JsonLdTypes.BTO);

            var activityObjectClone = details.ObjectWithBtoBcc.DeepClone();
            var activityObjectMainObjectCloneResult = activityObjectClone
                .Single()
                .As<JObject>();

            // strip bcc, bto from activity object
            if (activityObjectMainObjectCloneResult.ContainsKey(JsonLdTypes.BCC))
                activityObjectMainObjectCloneResult.Remove(JsonLdTypes.BCC);
            if (activityObjectMainObjectCloneResult.ContainsKey(JsonLdTypes.BTO))
                activityObjectMainObjectCloneResult.Remove(JsonLdTypes.BTO);

            // dereference activity object
            activityMainObjectCloneResult[JsonLdTypes.OBJECT] = activityObjectClone;

            return activityClone.As<JArray>();
        }

    }
}
