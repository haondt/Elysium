using Elysium.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.ActivityPub.Models
{
    public static class ActivityPubConsts
    {
        public static RemoteIri PUBLIC_COLLECTION = new RemoteIri { Iri = Iri.FromUnencodedString("https://www.w3.org/ns/activitystreams#Public") };
    }
}
