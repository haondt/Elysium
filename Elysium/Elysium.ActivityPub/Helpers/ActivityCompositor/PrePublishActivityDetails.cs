using Elysium.ActivityPub.Models;
using Elysium.Core.Extensions;
using Newtonsoft.Json.Linq;

namespace Elysium.ActivityPub.Helpers.ActivityCompositor
{
    public class PrePublishActivityDetails : ICompositionDetails
    {
        public required JArray ReferencedActivityWithBtoBcc { get; set; }
        public required JArray ObjectWithBtoBcc { get; set; }

        public JArray Composit()
        {
            var activityClone = ReferencedActivityWithBtoBcc.DeepClone();
            var activityMainObjectCloneResult = activityClone
                .Single()
                .As<JObject>();

            // strip bcc, bto from activity
            if (activityMainObjectCloneResult.ContainsKey(JsonLdTypes.BCC))
                activityMainObjectCloneResult.Remove(JsonLdTypes.BCC);
            if (activityMainObjectCloneResult.ContainsKey(JsonLdTypes.BTO))
                activityMainObjectCloneResult.Remove(JsonLdTypes.BTO);

            var activityObjectClone = ObjectWithBtoBcc.DeepClone();
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
