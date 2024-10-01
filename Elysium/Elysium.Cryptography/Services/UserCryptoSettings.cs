using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Cryptography.Services
{
    public class UserCryptoSettings
    {
        public string DefaultPbkdf2HashAlgorithm { get; set; } = HashAlgorithmName.SHA256.Name!;
        public int DefaultPbkdf2Iterations { get; set; } = 10000;
        public int DefaultPbkdf2SaltSizeInBytes { get; set; } = 16;
        public int DefaultEncryptionKeySizeInBits { get; set; } = 256;

        public int DefaultRSAKeySizeInBits { get; set; } = 2048;

        public int DefaultDocumentIdSizeInBytes { get; set; } = 8;

        public required EncryptionKeySettings ActorDataEncryptionKeySettings { get; set; }
    }
    public class EncryptionKeySettings
    {
        public required int CurrentVersion { get; set; }
        /// <summary>
        /// Version: Key dictionary of encryption keys.
        /// Keys should be 128 (16), 192 (24) or 256 (32) bits (bytes) in length, base64 encoded.
        /// </summary>
        public required Dictionary<int, string> Keys { get; set; }
    }
}
