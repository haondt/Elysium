using Elysium.Core.Models;
using Elysium.GrainInterfaces.Services;
using Haondt.Identity.StorageKey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces
{
    [GenerateSerializer]
    public class LocalActorState
    {
        [Id(0)]
        public required LocalIri Inbox { get; set; }

        public static StorageKey<LocalActorState> CreateStorageKey(LocalIri iri) => StorageKey<LocalActorState>.Create(iri.Iri.ToString());
    }
}
