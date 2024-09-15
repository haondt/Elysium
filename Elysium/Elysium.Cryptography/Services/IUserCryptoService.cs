namespace Elysium.Cryptography.Services
{
    public interface IUserCryptoService
    {
        (string PublicKey, string EncryptedPrivateKey) GenerateKeyPair();
        byte[] DecryptPrivateKey(string encryptedPrivateKey);
        string Sign(string data, byte[] privateKey);
        bool VerifySignature(string data, string signature, string publicKey);
        //string EncodePublicKeyAsPemX509(byte[] publicKey);
        byte[] DecodePublicKeyFromPemX509(string publicKey);
        string EncodePublicKeyToPemX509(string publicKey);
        //string EncodeMultibaseString(byte[] bytes);
        byte[] DecodeMultibaseString(string input);
        string GenerateDocumentId();
    }
}
