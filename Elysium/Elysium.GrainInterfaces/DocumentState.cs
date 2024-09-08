using Elysium.GrainInterfaces.Services;
using Elysium.Hosting.Models;
using Newtonsoft.Json.Linq;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces
{
    [Immutable]
    [Serializable]
    public class DocumentState
    {
        public JObject? Value { get; set; }
        public LocalUri? Owner { get; set; }
        public DateTime? UpdatedOnUtc { get; set; }
    }
}
