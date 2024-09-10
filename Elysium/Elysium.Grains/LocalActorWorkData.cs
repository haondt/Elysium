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
        public List<Iri> Recipients { get; set; } = [];
        public required JObject Acivity { get; set; }
    }
}
