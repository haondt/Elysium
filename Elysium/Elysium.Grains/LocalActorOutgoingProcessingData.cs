using Elysium.ActivityPub.Models;
using Elysium.Core.Models;
using Newtonsoft.Json.Linq;

namespace Elysium.Domain
{
    [GenerateSerializer, Immutable]
    public class LocalActorOutgoingProcessingData
    {
        /// <summary>
        /// Each iri in this list is either a target or a collection. if it is a collection, you must resolve it recursively
        /// </summary>
        [Id(0)]
        public List<Iri> Recipients { get; set; } = [];
        [Id(1)]
        public required JObject Activity { get; set; }
        [Id(2)]
        public required LocalIri ActivityIri { get; set; }
        [Id(3)]
        public required ActivityType ActivityType { get; set; }
    }
}
