using Elysium.Core.Models;
using Elysium.Cryptography.Services;
using FluentAssertions;
using Microsoft.Extensions.Options;
using System.Text;

namespace Elysium.Cryptography.Tests
{
    public class UserCryptoServiceTests
    {
        private readonly UserCryptoService _sut;
        private readonly UserCryptoService _sut2;

        public UserCryptoServiceTests()
        {
            _sut = new UserCryptoService(new CryptoService(), Options.Create(new UserCryptoSettings
            {
                ActorDataEncryptionKeySettings = new EncryptionKeySettings
                {
                    CurrentVersion = 1,
                    Keys = new Dictionary<int, string>
                    {
                        { 1, "gBrgKxxqAYegP3Kr0/R76Q==" },
                    }
                }
            }));

            _sut2 = new UserCryptoService(new CryptoService(), Options.Create(new UserCryptoSettings
            {
                ActorDataEncryptionKeySettings = new EncryptionKeySettings
                {
                    CurrentVersion = 2,
                    Keys = new Dictionary<int, string>
                    {
                        { 1, "gBrgKxxqAYegP3Kr0/R76Q==" },
                        { 2, "xnO7+hE4qgSyXc1DRMcu3BK0g1pMUNYGDvCJCYmpVrI=" }
                    }
                }
            }));
        }

        [Fact]
        public void WillEncryptAndDecryptActorData()
        {
            var signingKey = "my signing key";
            var signingKeyBytes = Encoding.UTF8.GetBytes(signingKey);
            var userIri = new LocalIri { Iri = new IriBuilder { Host = "example.com", Path = "users/testUser" }.Iri };
            var encryptedData = _sut.EncryptCryptographicActorData(new PlaintextCryptographicActorData
            {
                SigningKey = signingKeyBytes,
            }, userIri);
            encryptedData.EncryptedSigningKey.Should().NotBeEquivalentTo(signingKey);

            var plaintextData = _sut.DecryptCryptographicActorData(encryptedData, userIri);
            plaintextData.SigningKey.Should().BeEquivalentTo(signingKeyBytes);
        }

        [Fact]
        public void WillDecryptDataEncryptedWithOldMasterKey()
        {
            var signingKey = "my signing key";
            var signingKeyBytes = Encoding.UTF8.GetBytes(signingKey);
            var userIri = new LocalIri { Iri = new IriBuilder { Host = "example.com", Path = "users/testUser" }.Iri };
            var encryptedData = _sut.EncryptCryptographicActorData(new PlaintextCryptographicActorData
            {
                SigningKey = signingKeyBytes,
            }, userIri);
            encryptedData.EncryptedSigningKey.Should().NotBeEquivalentTo(signingKey);

            var plaintextData = _sut2.DecryptCryptographicActorData(encryptedData, userIri);
            plaintextData.SigningKey.Should().BeEquivalentTo(signingKeyBytes);
        }

        [Fact]
        public void WillRotateMasterKey()
        {
            var signingKey = "my signing key";
            var signingKeyBytes = Encoding.UTF8.GetBytes(signingKey);
            var userIri = new LocalIri { Iri = new IriBuilder { Host = "example.com", Path = "users/testUser" }.Iri };
            var encryptedData = _sut.EncryptCryptographicActorData(new PlaintextCryptographicActorData
            {
                SigningKey = signingKeyBytes,
            }, userIri);

            var rotatedEncryptedData = _sut2.ReEncryptCryptographicActorData(encryptedData, userIri);

            PlaintextCryptographicActorData? failedPlaintextData = null;
            try
            {
                failedPlaintextData = _sut.DecryptCryptographicActorData(rotatedEncryptedData, userIri);
            }
            catch { }

            if (failedPlaintextData != null) 
                failedPlaintextData.SigningKey.Should().NotBeEquivalentTo(signingKeyBytes);


            var plaintextData = _sut2.DecryptCryptographicActorData(rotatedEncryptedData, userIri);
            plaintextData.SigningKey.Should().BeEquivalentTo(signingKeyBytes);
        }

    }
}