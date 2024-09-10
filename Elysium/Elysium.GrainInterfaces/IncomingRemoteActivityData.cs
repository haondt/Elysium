using Elysium.Core.Models;

namespace Elysium.GrainInterfaces
{
    [GenerateSerializer]
    public class IncomingRemoteActivityData
    {
        public required string Payload { get; set; }
        public required LocalIri Target { get; set; }
        public required List<(string Key, string Value)> Headers { get; set; } = [];
    }
}