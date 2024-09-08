using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.ActivityPub.Helpers.ActivityCompositor
{
    public class PrePublishActivityDetails
    {
        public required JArray ReferencedActivityWithBtoBcc { get; set; }
        public required JArray ObjectWithBtoBcc { get; set; }
    }
}
