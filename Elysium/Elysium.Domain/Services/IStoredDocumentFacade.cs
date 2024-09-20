using Elysium.Core.Models;
using Elysium.GrainInterfaces;
using Haondt.Core.Models;
using Haondt.Persistence.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
