using Elysium.Core.Models;

namespace Elysium.ActivityPub.Models
{
    public static class ActivityPubConsts
    {
        public static RemoteIri PUBLIC_COLLECTION = new RemoteIri { Iri = Iri.FromUnencodedString("https://www.w3.org/ns/activitystreams#Public") };
    }
}
