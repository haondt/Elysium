using Elysium.Core.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains
{
    [GenerateSerializer, Immutable]
    public class LocalActorWorkData
    {
        [Id(0)]
        public List<Iri> Recipients { get; set; } = [];
        [Id(1)]
        public required JObject Acivity { get; set; }
    }
}
