namespace Elysium.GrainInterfaces.Services
{
    [GenerateSerializer, Immutable]
    public class ActorRegistrationDetails
    {
        [Id(0)]
        public required byte[] PrivateKey { get; set; }
        [Id(1)]
        public required string PublicKey { get; set; }
        [Id(2)]
        public required string Type { get; set; }
    }
}
