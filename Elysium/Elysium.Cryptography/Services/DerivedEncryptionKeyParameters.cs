namespace Elysium.Cryptography.Services
{
    [GenerateSerializer]
    public class DerivedEncryptionKeyParameters
    {
        [Id(0)]
        public required string Salt { get; set; }
        [Id(1)]
        public required int Iterations { get; set; }
        [Id(2)]
        public required string HashAlgorithmName { get; set; }
        [Id(3)]
        public required int SizeInBits { get; set; }
    }
}
