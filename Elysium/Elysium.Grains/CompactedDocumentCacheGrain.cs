using Elysium.GrainInterfaces;
using Newtonsoft.Json.Linq;

namespace Elysium.Domain
{
    public class CompactedDocumentCacheGrain : CacheGrain<DocumentState<JObject>>, ICompactedDocumentCacheGrain
    {
    }
}
