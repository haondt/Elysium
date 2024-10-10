using Elysium.Core.Models;

namespace Elysium.GrainInterfaces.RemoteActor
{
    [GenerateSerializer, Immutable]
    public class DispatchRemoteActivityData
    {
        public required string Payload { get; set; }
        public required LocalIri Sender { get; set; }
        public required RemoteIri Target { get; set; }
    }
}
