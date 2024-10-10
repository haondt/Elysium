namespace Elysium.Grains.InstanceActor
{
    public class InstanceActorSettings
    {
        public required string PrivateKey { get; set; }
        public required string PublicKey { get; set; }
        public bool SignRequests { get; set; }
    }
}