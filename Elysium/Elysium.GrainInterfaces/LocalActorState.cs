using Elysium.Core.Models;
using Elysium.Cryptography.Services;
using Haondt.Identity.StorageKey;

namespace Elysium.GrainInterfaces
{
    [GenerateSerializer]
    public class LocalActorState
    {
        [Id(2)]
        public bool IsInitialized { get; set; } = false;
        [Id(3)]
        public required EncryptedCryptographicActorData CryptographicActorData { get; set; }
        [Id(4)]
        public required string PublicKey { get; set; }
        [Id(5)]
        public required string Type { get; set; }
        [Id(6)]
        public required LocalIri Id { get; set; }


        public static StorageKey<LocalActorState> CreateStorageKey(LocalIri iri) => StorageKey<LocalActorState>.Create(iri.Iri.ToString());
    }
}
