namespace Elysium.GrainInterfaces
{
    public class IncomingRemoteActivityData
    {
        public required string Payload { get; set; }
        public required Uri Target { get; set; }
        public required List<(string Key, string Value)> Headers { get; set; } = [];
    }
}