using Elysium.GrainInterfaces;
using Newtonsoft.Json.Linq;

namespace Elysium.Domain
{
    public class ExpandedDocumentCacheGrain : CacheGrain<DocumentState<JArray>>, IExpandedDocumentCacheGrain
    {
    }
}
