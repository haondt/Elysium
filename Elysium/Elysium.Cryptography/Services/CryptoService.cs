using Elysium.Core.Models;
using Microsoft.Extensions.Options;
using SimpleBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Cryptography.Services
{
    public class CryptoService() : ICryptoService
    {

        public (byte[] EncryptedData, byte[] IV)  AesEncrypt(byte[] data, byte[] encryptionKey)
        {
            using Aes aes = Aes.Create();
            aes.Key = encryptionKey;
            aes.GenerateIV();

            var encryptor = aes.CreateEncryptor();
            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
            cs.Write(data, 0, data.Length);
            cs.FlushFinalBlock();

            return (ms.ToArray(), aes.IV);
        }

        public byte[] AesDecrypt(byte[] data, byte[] encryptionKey, byte[] iv)
        {
            using Aes aes = Aes.Create();
            aes.Key = encryptionKey;
            aes.IV = iv;

            var decryptor = aes.CreateDecryptor();
            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write);
            cs.Write(data, 0, data.Length);
            cs.FlushFinalBlock();

            return ms.ToArray();
        }

        public (byte[] PublicKey, byte[] PrivateKey) GenerateRSAKeyPair(int keySizeInBits)
        {
            using var rsa = RSA.Create(keySizeInBits);
            var publicKey = rsa.ExportRSAPublicKey();
            var privateKey = rsa.ExportRSAPrivateKey();
            return (publicKey, privateKey);
        }

        public byte[] GenerateAesEncryptionKey(int keySizeInBits)
        {
            using var aes = Aes.Create();
            aes.KeySize = keySizeInBits;
            aes.GenerateKey();
            return aes.Key;
        }

        public byte[] DerivePbkdf2EncryptionKey(
            byte[] sourceBytes,
            byte[] salt,
            int iterations,
            HashAlgorithmName hashAlgorithmName,
            int encryptionKeySizeInBits)
        {
            return Rfc2898DeriveBytes.Pbkdf2(sourceBytes, salt, iterations, hashAlgorithmName, encryptionKeySizeInBits / 8);
        }

        public byte[] CreateRSASignature(byte[] data, byte[] privateKey)
        {
            using var rsa = RSA.Create();
            return rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }

        public byte[] GenerateRandomBytes(int num)
        {
            return RandomNumberGenerator.GetBytes(num);
        }

        public bool VerifyRSASignature(byte[] data, byte[] signature, byte[] publicKey)
        {
            using var rsa = RSA.Create();
            return rsa.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }

    }
}
