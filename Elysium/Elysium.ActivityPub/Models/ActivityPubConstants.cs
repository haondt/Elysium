using Elysium.Core.Models;

namespace Elysium.ActivityPub.Models
{
    public static class ActivityPubConstants
    {
        public static RemoteIri PUBLIC_COLLECTION = new() { Iri = Iri.FromUnencodedString("https://www.w3.org/ns/activitystreams#Public") };
    }
}
