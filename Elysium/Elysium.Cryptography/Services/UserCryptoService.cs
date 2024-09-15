using Elysium.Cryptography.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SimpleBase;
using System.Security.Cryptography;
using System.Text;

namespace Elysium.Authentication.Services
{
    public class UserCryptoService(ICryptoService cryptoService,
        IDataProtectionProvider dataProtector) : IUserCryptoService
    {
        private readonly IDataProtector _dataProtector = dataProtector.CreateProtector($"{nameof(UserCryptoService)}.v1");
        public byte[] DecryptPrivateKey(string encryptedPrivateKey)
        {
            return _dataProtector.Unprotect(Convert.FromBase64String(encryptedPrivateKey));
            //return cryptoService.AesDecrypt(Convert.FromBase64String(encryptedPrivateKey), _encryptionKey, _iv);
        }
        public (string PublicKey, string EncryptedPrivateKey) GenerateKeyPair()
        {

            var (publicKey, privateKey) = cryptoService.GenerateKeyPair();
            var encryptedPrivateKey = _dataProtector.Protect(publicKey);

            //var encryptedPrivateKey = cryptoService.AesEncrypt(privateKey, _encryptionKey, _iv);

            return (Convert.ToBase64String(publicKey), Convert.ToBase64String(encryptedPrivateKey));
        }

        public string Sign(string data, byte[] privateKey)
        {
            var signatureBytes = cryptoService.Sign(Encoding.UTF8.GetBytes(data), privateKey);
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

        // generate a random 64 bit object id, encoded as a url-safe base64 string
        public string GenerateDocumentId()
        {
            var bytes = cryptoService.GenerateRandomBytes(8);
            return Base64UrlEncoder.Encode(bytes);
        }

        //public string EncodePublicKeyAsPemX509(byte[] publicKey)
        //{
        //    using var rsa = RSA.Create();
        //    rsa.ImportRSAPublicKey(publicKey, out _);
        //    return rsa.ExportSubjectPublicKeyInfoPem();
        //}

        // https://www.w3.org/TR/controller-document/#multibase-0
        public byte[] DecodeMultibaseString(string input)
        {
            if (input.Length < 1)
                throw new InvalidOperationException("input must be at least 1 char long");

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
            throw new InvalidOperationException("input in an unrecognized format");
        }

        public byte[] DecodePublicKeyFromPemX509(string publicKey)
        {
            using var rsa = RSA.Create();
            rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(publicKey), out _);
            return rsa.ExportRSAPublicKey();
        }

        public string EncodePublicKeyToPemX509(string publicKey)
        {
            using var rsa = RSA.Create();
            rsa.ImportRSAPublicKey(Convert.FromBase64String(publicKey), out _);
            return rsa.ExportSubjectPublicKeyInfoPem();
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
