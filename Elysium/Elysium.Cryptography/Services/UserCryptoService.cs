using Elysium.Core.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SimpleBase;
using System.Security.Cryptography;
using System.Text;

namespace Elysium.Cryptography.Services
{
    public class UserCryptoService(ICryptoService cryptoService, IOptions<UserCryptoSettings> options) : IUserCryptoService
    {
        private readonly UserCryptoSettings _settings = options.Value;

        public EncryptedCryptographicActorData ReEncryptCryptographicActorData(EncryptedCryptographicActorData data, LocalIri actorIri)
        {
            if (!ShouldUpdateMasterKeyVersion(data))
                return data;

            var (derivedKey, _, _) = DeriveEncryptionKeyFromActorIri(actorIri, data.MasterKeyVersion);
            var encryptionKey = cryptoService.AesDecrypt(
                Convert.FromBase64String(data.EncryptedEncryptionKey),
                derivedKey,
                Convert.FromBase64String(data.EncryptedEncryptionKeyParameters.IV));

            var (newDerivedKey, derivedKeyParameters, masterKeyVersion) = DeriveEncryptionKeyFromActorIri(actorIri);
            var (encryptedEncryptionKey, encryptedEncryptionKeyIV) = cryptoService.AesEncrypt(encryptionKey, newDerivedKey);

            return new EncryptedCryptographicActorData
            {
                MasterKeyVersion = masterKeyVersion,
                DerivedKeyParameters = derivedKeyParameters,

                EncryptedEncryptionKey = Convert.ToBase64String(encryptedEncryptionKey),
                EncryptedEncryptionKeyParameters = new EncryptedDataParameters
                {
                    IV = Convert.ToBase64String(encryptedEncryptionKeyIV),
                },

                EncryptedSigningKey = data.EncryptedSigningKey,
                EncryptedSigningKeyParameters = data.EncryptedSigningKeyParameters
            };
        }

        public PlaintextCryptographicActorData DecryptCryptographicActorData(EncryptedCryptographicActorData data, LocalIri actorIri)
        {
            var (derivedKey, _, _) = DeriveEncryptionKeyFromActorIri(actorIri, data.MasterKeyVersion);
            var encryptionKey = cryptoService.AesDecrypt(
                Convert.FromBase64String(data.EncryptedEncryptionKey),
                derivedKey,
                Convert.FromBase64String(data.EncryptedEncryptionKeyParameters.IV));
            var signingKey = cryptoService.AesDecrypt(
                Convert.FromBase64String(data.EncryptedSigningKey),
                encryptionKey,
                Convert.FromBase64String(data.EncryptedSigningKeyParameters.IV));

            return new PlaintextCryptographicActorData
            {
                SigningKey = signingKey
            };
        }

        public EncryptedCryptographicActorData EncryptCryptographicActorData(PlaintextCryptographicActorData data, LocalIri actorIri)
        {
            var (derivedKey, derivedKeyParameters, masterKeyVersion) = DeriveEncryptionKeyFromActorIri(actorIri);

            var encryptionKey = GenerateEncryptionKey();
            var (encryptedEncryptionKey, encryptedEncryptionKeyIV) = cryptoService.AesEncrypt(encryptionKey, derivedKey);

            var (encryptedSigningKey, encryptedSigningKeyIV) = cryptoService.AesEncrypt(data.SigningKey, encryptionKey);

            return new EncryptedCryptographicActorData
            {
                MasterKeyVersion = masterKeyVersion,
                DerivedKeyParameters = derivedKeyParameters,

                EncryptedEncryptionKey = Convert.ToBase64String(encryptedEncryptionKey),
                EncryptedEncryptionKeyParameters = new EncryptedDataParameters
                {
                    IV = Convert.ToBase64String(encryptedEncryptionKeyIV),
                },

                EncryptedSigningKey = Convert.ToBase64String(encryptedSigningKey),
                EncryptedSigningKeyParameters = new EncryptedDataParameters
                {
                    IV = Convert.ToBase64String(encryptedSigningKeyIV)
                }
            };
        }

        public bool ShouldUpdateMasterKeyVersion(EncryptedCryptographicActorData data)
        {
            return data.MasterKeyVersion != _settings.ActorDataEncryptionKeySettings.CurrentVersion;
        }

        private (byte[] EncryptionKey, DerivedEncryptionKeyParameters Parameters, int MasterKeyVersion) DeriveEncryptionKeyFromActorIri(LocalIri actorIri, int? preferredMasterKeyVersion = null)
        {
            var masterKeyVersion = preferredMasterKeyVersion ?? _settings.ActorDataEncryptionKeySettings.CurrentVersion;
            var masterKey = Convert.FromBase64String(_settings.ActorDataEncryptionKeySettings.Keys[masterKeyVersion]);

            var salt = Encoding.UTF8.GetBytes(actorIri.ToString());
            var parameters = new DerivedEncryptionKeyParameters
            {
                Salt = Convert.ToBase64String(salt),
                Iterations = _settings.DefaultPbkdf2Iterations,
                HashAlgorithmName = _settings.DefaultPbkdf2HashAlgorithm,
                SizeInBits = _settings.DefaultEncryptionKeySizeInBits,
            };
            return (cryptoService.DerivePbkdf2EncryptionKey(
                masterKey,
                salt,
                parameters.Iterations,
                new HashAlgorithmName(parameters.HashAlgorithmName),
                parameters.SizeInBits), parameters, masterKeyVersion);
        }


        private byte[] GenerateEncryptionKey() => cryptoService.GenerateAesEncryptionKey(_settings.DefaultEncryptionKeySizeInBits);

        public (string PublicKey, byte[] PrivateKey) GenerateKeyPair()
        {
            var (publicKey, privateKey) = cryptoService.GenerateRSAKeyPair(_settings.DefaultRSAKeySizeInBits);
            return (Convert.ToBase64String(publicKey), privateKey);
        }

        public string Sign(string data, byte[] privateKey)
        {
            var signatureBytes = cryptoService.CreateRSASignature(Encoding.UTF8.GetBytes(data), privateKey);
            return Convert.ToBase64String(signatureBytes);
        }

        public bool VerifySignature(string data, string signature, string publicKey)
        {
            try
            {
                return cryptoService.VerifyRSASignature(
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
            var bytes = cryptoService.GenerateRandomBytes(_settings.DefaultDocumentIdSizeInBytes);
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
                switch (input.Length % 4)
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
