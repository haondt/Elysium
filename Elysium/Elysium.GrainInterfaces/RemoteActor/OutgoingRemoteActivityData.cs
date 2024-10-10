using Elysium.Core.Models;

namespace Elysium.GrainInterfaces.RemoteActor
{
    [GenerateSerializer, Immutable]
    public class OutgoingRemoteActivityData
    {
        public required string Payload { get; set; }
        public required LocalIri Sender { get; set; }
    }
}
