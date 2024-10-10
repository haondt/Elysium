using Elysium.ActivityPub.Models;
using Elysium.Core.Models;
using Newtonsoft.Json.Linq;

namespace Elysium.Grains.LocalActor
{
    [GenerateSerializer, Immutable]
    public class LocalActorIncomingProcessingData
    {
        [Id(0)]
        public required JToken Activity { get; set; }
        [Id(1)]
        public required Iri Sender { get; set; }
        [Id(2)]
        public required ActivityType ActivityType { get; set; }
    }
}
