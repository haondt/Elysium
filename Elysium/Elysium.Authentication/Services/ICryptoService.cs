using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Authentication.Services
{
    public interface ICryptoService
    {
        (byte[] PublicKey, byte[] PrivateKey) GenerateKeyPair();
        byte[] AesEncrypt(byte[] data, byte[] encryptionKey, byte[] iv);
        byte[] AesDecrypt(byte[] data, byte[] encryptionKey, byte[] iv);
        byte[] Sign(byte[] data, byte[] privateKey);
        bool VerifySignature(byte[] data, byte[] signature, byte[] publicKey);
        byte[] GenerateRandomBytes(int num);
    }
}
