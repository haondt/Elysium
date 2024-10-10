using Elysium.Core.Models;
using Elysium.GrainInterfaces.Documents;
using Haondt.Core.Models;
using Haondt.Persistence.Services;
using Newtonsoft.Json.Linq;

namespace Elysium.Domain.Services
{
    public interface IStoredDocumentFacade
    {
        Task ClearAsync(Iri iri);
        Task<bool> ExistsAsync(Iri iri);
        Task<Result<DocumentState, StorageResultReason>> GetAsync(Iri iri);
        Task<Result<DocumentState<JObject>, StorageResultReason>> GetCompactedAsync(Iri iri);
        Task<Result<DocumentState<JArray>, StorageResultReason>> GetExpandedAsync(Iri iri);
        Task SetAsync(Iri iri, DocumentState value);
    }
}
