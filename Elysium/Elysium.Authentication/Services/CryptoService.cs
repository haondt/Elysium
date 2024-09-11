using SimpleBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Authentication.Services
{
    public class CryptoService : ICryptoService
    {
        public byte[] AesEncrypt(byte[] data, byte[] encryptionKey, byte[] iv)
        {
            using Aes aes = Aes.Create();
            aes.Key = encryptionKey;
            aes.IV = iv;

            var encryptor = aes.CreateEncryptor();
            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
            using var sw = new StreamWriter(cs);

            sw.Write(data);
            return ms.ToArray();
        }

        public byte[] AesDecrypt(byte[] data, byte[] encryptionKey, byte[] iv)
        {
            using Aes aes = Aes.Create();
            aes.Key = encryptionKey;
            aes.IV = iv;

            var decryptor = aes.CreateDecryptor();
            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sw = new StreamWriter(cs);

            sw.Write(data);
            return ms.ToArray();
        }

        public (byte[] PublicKey, byte[] PrivateKey) GenerateKeyPair()
        {
            using var rsa = RSA.Create(2048);
            var publicKey = rsa.ExportRSAPublicKey();
            var privateKey = rsa.ExportRSAPrivateKey();
            return(publicKey, privateKey);
        }

        public byte[] Sign(byte[] data, byte[] privateKey)
        {
            using var rsa = RSA.Create();
            return rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }
        
        public byte[] GenerateRandomBytes(int num)
        {
            return RandomNumberGenerator.GetBytes(num);
        }

        public bool VerifySignature(byte[] data, byte[] signature, byte[] publicKey)
        {
            using var rsa = RSA.Create();
            return rsa.VerifyData(data, signature,HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }

    }
}
