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
        public static StorageKey<DocumentState> GetStorageKey(Iri iri) => StorageKey<DocumentState>.Create(iri.ToString());

        [Id(0)]
        public JToken? Value { get; set; }
        [Id(1)]
        public LocalIri? Owner { get; set; }
        [Id(2)]
        public DateTime? UpdatedOnUtc { get; set; }
        [Id(3)]
        public List<Iri> BTo { get; set; } = [];
        [Id(4)]
        public List<Iri> BCc { get; set; } = [];
        [Id(5)]
        public bool IsReservation { get; set; } = false;
    }

    [GenerateSerializer]
    public class DocumentState<T> where T : JToken
    {
        [Id(0)]
        public T? Value { get; set; }
        [Id(1)]
        public LocalIri? Owner { get; set; }
        [Id(2)]
        public DateTime? UpdatedOnUtc { get; set; }
        [Id(3)]
        public List<Iri> BTo { get; set; } = [];
        [Id(4)]
        public List<Iri> BCc { get; set; } = [];
        [Id(5)]
        public bool IsReservation { get; set; } = false;
    }
}
