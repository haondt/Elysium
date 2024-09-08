using DotNext;
using Elysium.ActivityPub.Helpers;
using Elysium.ActivityPub.Helpers.ActivityCompositor;
using Elysium.ActivityPub.Models;
using Elysium.Core.Extensions;
using Newtonsoft.Json.Linq;

namespace Elysium.ActivityPub.Helpers.ActivityCompositor
{
    public static class ActivityCompositor
    {
        public static Result<JArray> Composit(ICompositionDetails details)
        {
            switch (details)
            {
                case MessageDetails messageDetails:
                    return new ActivityPubJsonBuilder()
                        .Type(messageDetails.Type)
                        .To(messageDetails.Recepient)
                        .Published(DateTime.UtcNow)
                        .Content(messageDetails.Text)
                        .Build();
                case CreateActivityDetails activityDetails:
                    return new ActivityPubJsonBuilder()
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

            return new (new InvalidOperationException($"Unkown object details type {details.GetType()}"));
        }

        public static Result<JArray> Composit(PrePublishActivityDetails details)
        {
            var activityClone = details.ReferencedActivityWithBtoBcc.DeepClone();
            var activityMainObjectCloneResult = new Result<JToken>(activityClone)
                .Single()
                .As<JObject>();
            if (!activityMainObjectCloneResult.IsSuccessful)
                return new(activityMainObjectCloneResult.Error);

            // strip bcc, bto from activity
            if (activityMainObjectCloneResult.Value.ContainsKey(JsonLdTypes.BCC))
                activityMainObjectCloneResult.Value.Remove(JsonLdTypes.BCC);
            if (activityMainObjectCloneResult.Value.ContainsKey(JsonLdTypes.BTO))
                activityMainObjectCloneResult.Value.Remove(JsonLdTypes.BTO);

            var activityObjectClone = details.ObjectWithBtoBcc.DeepClone();
            var activityObjectMainObjectCloneResult = new Result<JToken>(activityObjectClone)
                .Single()
                .As<JObject>();
            if (!activityObjectMainObjectCloneResult.IsSuccessful)
                return new(activityObjectMainObjectCloneResult.Error);

            // strip bcc, bto from activity object
            if (activityObjectMainObjectCloneResult.Value.ContainsKey(JsonLdTypes.BCC))
                activityObjectMainObjectCloneResult.Value.Remove(JsonLdTypes.BCC);
            if (activityObjectMainObjectCloneResult.Value.ContainsKey(JsonLdTypes.BTO))
                activityObjectMainObjectCloneResult.Value.Remove(JsonLdTypes.BTO);

            // dereference activity object
            var setResult = activityMainObjectCloneResult.Set(JsonLdTypes.OBJECT, activityObjectClone);
            if (setResult.HasValue)
                return new(setResult.Value);

            return new Result<JToken>(activityClone).As<JArray>();
        }
    }
}
