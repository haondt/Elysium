using Elysium.Core.Models;
using Haondt.Identity.StorageKey;

namespace Elysium.Client.Services
{
    [GenerateSerializer]
    public class UserState
    {
        public static StorageKey<UserState> CreateStorageKey(LocalIri iri) => StorageKey<UserState>.Create(iri.Iri.ToString());

        [Id(0)]
        public List<LocalIri> Shades { get; set; } = [];
    }
}