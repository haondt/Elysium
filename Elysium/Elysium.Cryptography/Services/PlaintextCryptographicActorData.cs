namespace Elysium.Cryptography.Services
{
    [GenerateSerializer]
    public class PlaintextCryptographicActorData
    {
        [Id(0)]
        public required byte[] SigningKey { get; set; }
    }
}
