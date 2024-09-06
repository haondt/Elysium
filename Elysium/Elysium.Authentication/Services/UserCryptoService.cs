using DotNext;
using Microsoft.Extensions.Options;
using SimpleBase;
using System.Security.Cryptography;

namespace Elysium.Authentication.Services
{
    public class UserCryptoService(IOptions<CryptoSettings> options, ICryptoService cryptoService) : IUserCryptoService
    {
        private readonly byte[] _encryptionKey = Convert.FromBase64String(options.Value.EncryptionKey);
        private readonly byte[] _iv = Convert.FromBase64String(options.Value.IV);
        public byte[] DecryptPrivateKey(string encryptedPrivateKey)
        {
            return cryptoService.AesDecrypt(Convert.FromBase64String(encryptedPrivateKey), _encryptionKey, _iv);
        }
        public (string PublicKey, string EncryptedPrivateKey) GenerateKeyPair()
        {

            var (publicKey, privateKey) = cryptoService.GenerateKeyPair();
            var encryptedPrivateKey = cryptoService.AesEncrypt(privateKey, _encryptionKey, _iv);

            return (Convert.ToBase64String(publicKey), Convert.ToBase64String(encryptedPrivateKey));
        }

        public string Sign(string data, byte[] privateKey)
        {
            var signatureBytes = cryptoService.Sign(Convert.FromBase64String(data), _encryptionKey);
            return Convert.ToBase64String(signatureBytes);
        }

        public bool VerifySignature(string data, string signature, string publicKey)
        {
            try
            {
                return cryptoService.VerifySignature(
                    Convert.FromBase64String(data),
                    Convert.FromBase64String(signature),
                    Convert.FromBase64String(publicKey));
            }
            catch
            {
                return false;
            }
        }
        //public string EncodePublicKeyAsPemX509(byte[] publicKey)
        //{
        //    using var rsa = RSA.Create();
        //    rsa.ImportRSAPublicKey(publicKey, out _);
        //    return rsa.ExportSubjectPublicKeyInfoPem();
        //}

        // https://www.w3.org/TR/controller-document/#multibase-0
        public Result<byte[]> DecodeMultibaseString(string input)
        {
            try
            {
                if (input.Length < 1)
                    return new(new InvalidOperationException("input must be at least 1 char long"));

                if (input[0] == 'u')
                {
                    // base-64-url-no-pad
                    input = input.Substring(1).Replace('-', '+').Replace('_', '/');
                    switch(input.Length % 4)
                    {
                        case 2: input += "=="; break;
                        case 3: input += "="; break;
                    }
                    return Convert.FromBase64String(input);
                }

                // base-58-btc
                if (input[0] == 'z')
                    return Base58.Bitcoin.Decode(input.AsSpan(1));
            }
            catch (Exception ex)
            {
                return new(ex);
            }
            return new(new InvalidOperationException("input in an unrecognized format"));
        }

        public Result<byte[]> DecodePublicKeyFromPemX509(string publicKey)
        {
            try
            {
                using var rsa = RSA.Create();
                rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(publicKey), out _);
                return rsa.ExportRSAPublicKey();
            }
            catch (Exception ex)
            {
                return new(ex);
            }
        }

        //public string EncodeMultibaseString(byte[] bytes)
        //{
        //    using var rsa = RSA.Create();
        //    rsa.ImportRSAPublicKey(publicKey, out _);
        //    return $"z{Base58.Bitcoin.Encode(publicKey)}";
        //}

        //DotNext.Result<byte[]> IUserCryptoService.DecodeMultibaseString(string input)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
