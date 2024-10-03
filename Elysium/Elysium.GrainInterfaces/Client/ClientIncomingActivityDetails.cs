using Elysium.ActivityPub.Models;
using Elysium.Core.Models;
using Haondt.Core.Models;
using Newtonsoft.Json.Linq;

namespace Elysium.GrainInterfaces.Client
{
    [GenerateSerializer, Immutable]
    public class ClientIncomingActivityDetails
    {
        [Id(0)]
        public required JArray ExpandedObject { get; set; }
        [Id(1)]
        public required ActivityType Type { get; set; }
        [Id(2)]
        public required Iri Sender { get; set; }
        [Id(3)]
        public required LocalIri Receiver { get; set; }
        [Id(4)]
        public required Optional<string> SenderPreferredUsername { get; set; }
    }
}
