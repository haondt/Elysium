namespace Elysium.Cryptography.Services
{
    [GenerateSerializer]
    public class EncryptedDataParameters
    {
        [Id(0)]
        public required string IV { get; set; }
    }
}
