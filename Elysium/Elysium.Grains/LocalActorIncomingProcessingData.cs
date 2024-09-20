using Elysium.ActivityPub.Models;
using Elysium.Core.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Domain
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
