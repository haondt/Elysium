using Elysium.Core.Models;

namespace Elysium.Cryptography.Services
{
    public interface IUserCryptoService
    {
        string Sign(string data, byte[] privateKey);
        bool VerifySignature(string data, string signature, string publicKey);
        //string EncodePublicKeyAsPemX509(byte[] publicKey);
        byte[] DecodePublicKeyFromPemX509(string publicKey);
        string EncodePublicKeyToPemX509(string publicKey);
        //string EncodeMultibaseString(byte[] bytes);
        byte[] DecodeMultibaseString(string input);
        string GenerateDocumentId();
        (string PublicKey, byte[] PrivateKey) GenerateKeyPair();
        EncryptedCryptographicActorData EncryptCryptographicActorData(PlaintextCryptographicActorData data, LocalIri actorIri);
        PlaintextCryptographicActorData DecryptCryptographicActorData(EncryptedCryptographicActorData data, LocalIri actorIri);
        bool ShouldUpdateMasterKeyVersion(EncryptedCryptographicActorData data);
        EncryptedCryptographicActorData ReEncryptCryptographicActorData(EncryptedCryptographicActorData data, LocalIri actorIri);
    }
}
