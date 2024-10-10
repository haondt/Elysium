using Elysium.Core.Models;
using Haondt.Core.Models;
using Newtonsoft.Json.Linq;

namespace Elysium.GrainInterfaces.Documents
{
    public interface ICompactedDocumentCacheGrain : IGrain<Iri>
    {
        Task<Optional<DocumentState<JObject>>> TryGetValueAsync();
        Task SetValueAsync(DocumentState<JObject> value);
        Task ClearAsync();
    }
}
