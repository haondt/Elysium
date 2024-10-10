using Elysium.GrainInterfaces.Documents;
using Elysium.Grains.Generics;
using Newtonsoft.Json.Linq;

namespace Elysium.Grains.Documents
{
    public class ExpandedDocumentCacheGrain : CacheGrain<DocumentState<JArray>>, IExpandedDocumentCacheGrain
    {
    }
}
