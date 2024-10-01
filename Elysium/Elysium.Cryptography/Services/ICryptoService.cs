using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Cryptography.Services
{
    public interface ICryptoService
    {
        (byte[] PublicKey, byte[] PrivateKey) GenerateRSAKeyPair(int keySizeInBits);
        (byte[] EncryptedData, byte[] IV) AesEncrypt(byte[] data, byte[] encryptionKey);
        byte[] AesDecrypt(byte[] data, byte[] encryptionKey, byte[] iv);
        byte[] CreateRSASignature(byte[] data, byte[] privateKey);
        bool VerifyRSASignature(byte[] data, byte[] signature, byte[] publicKey);
        byte[] GenerateRandomBytes(int count);
        byte[] GenerateAesEncryptionKey(int keySizeInBits);
        byte[] DerivePbkdf2EncryptionKey(byte[] sourceBytes, byte[] salt, int iterations, HashAlgorithmName hashAlgorithmName, int encryptionKeySizeInBits);
    }
}
