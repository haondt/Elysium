using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains
{
    public class LocalActorWorkData
    {
        public List<Uri> Recipients { get; set; } = [];
        public required JObject Acivity { get; set; }
    }
}
