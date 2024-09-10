using Elysium.Core.Models;
using Elysium.GrainInterfaces.Services;
using Haondt.Identity.StorageKey;
using Newtonsoft.Json.Linq;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces
{
    [GenerateSerializer]
    public class DocumentState
    {
        public static StorageKey<DocumentState> GetStorageKey(LocalIri localUri) => StorageKey<DocumentState>.Create(localUri.Iri.ToString());
        // TODO: add bto & bcc fields
        public JObject? Value { get; set; } // compacted
        public LocalIri? Owner { get; set; }
        public DateTime? UpdatedOnUtc { get; set; }
        public List<Iri> BTo { get; set; } = [];
        public List<Iri> BCc { get; set; } = [];
    }
}
