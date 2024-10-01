using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Cryptography.Services
{
    [GenerateSerializer]
    public class EncryptedCryptographicActorData
    {
        [Id(0)]
        public required int MasterKeyVersion { get; set; }
        [Id(1)]
        public required DerivedEncryptionKeyParameters DerivedKeyParameters { get; set; }

        [Id(2)]
        public required string EncryptedEncryptionKey { get; set; }
        [Id(3)]
        public required EncryptedDataParameters EncryptedEncryptionKeyParameters { get; set; }

        [Id(4)]
        public required string EncryptedSigningKey { get; set; }
        [Id(5)]
        public required EncryptedDataParameters EncryptedSigningKeyParameters { get; set; }
    }
}
