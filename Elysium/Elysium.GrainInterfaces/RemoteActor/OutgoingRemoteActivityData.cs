using Elysium.Core.Models;
using Newtonsoft.Json.Linq;

namespace Elysium.GrainInterfaces.RemoteActor
{
    [GenerateSerializer, Immutable]
    public class OutgoingRemoteActivityData
    {
        public required JToken Payload { get; set; }
        public required LocalIri Sender { get; set; }
        public required RemoteIri Receiver { get; set; }
    }
}
