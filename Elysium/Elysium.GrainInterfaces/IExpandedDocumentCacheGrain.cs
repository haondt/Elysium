using Elysium.Core.Models;
using Haondt.Core.Models;
using Newtonsoft.Json.Linq;

namespace Elysium.GrainInterfaces
{
    public interface IExpandedDocumentCacheGrain : IGrain<Iri>
    {
        Task<Optional<DocumentState<JArray>>> TryGetValueAsync();
        Task SetValueAsync(DocumentState<JArray> value);
        Task ClearAsync();
    }
}
