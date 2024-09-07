using DotNext;
using Elysium.ActivityPub.Helpers;
using Elysium.ActivityPub.Models;
using Newtonsoft.Json.Linq;

namespace Elysium.ActivityPub
{
    public static class ActivityCompositor
    {
        public static Result<JArray> Composit(IActivityObjectDetails details)
        {
            if (details is MessageDetails messageDetails)
            {
                return new ActivityPubJsonBuilder()
                    .Type(JsonLdTypes.NOTE)
                    .To(messageDetails.Recepient)
                    .Published(DateTime.UtcNow)
                    .Content(messageDetails.Text)
                    .Build();
            }

            return new (new InvalidOperationException($"Unkown object details type {details.GetType()}"));
        }
    }
}
