using Elysium.GrainInterfaces;
using Haondt.Core.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Domain
{
    public class CompactedDocumentCacheGrain : CacheGrain<DocumentState<JObject>>, ICompactedDocumentCacheGrain
    {
    }
}
