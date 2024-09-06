
using DotNext;

namespace Elysium.Authentication.Services
{
    public interface IUserCryptoService
    {
        (string PublicKey, string EncryptedPrivateKey) GenerateKeyPair();
        byte[] DecryptPrivateKey(string encryptedPrivateKey);
        string Sign(string data, byte[] privateKey);
        bool VerifySignature(string data, string signature, string publicKey);
        //string EncodePublicKeyAsPemX509(byte[] publicKey);
        Result<byte[]> DecodePublicKeyFromPemX509(string publicKey);
        //string EncodeMultibaseString(byte[] bytes);
        Result<byte[]> DecodeMultibaseString(string input);

    }
}
