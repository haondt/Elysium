using Elysium.Core.Models;
using Elysium.GrainInterfaces.Reasons;
using Elysium.GrainInterfaces.Services;
using Haondt.Core.Models;
using Haondt.Persistence.Services;
using Newtonsoft.Json.Linq;
using Orleans;
using Orleans.Concurrency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces
{
    public interface ILocalDocumentGrain : IGrain<LocalIri>
    {
        public Task<bool> HasValueAsync();
        public Task InitializeValueAsync(LocalIri owner, JObject value);
        public Task<Result<DocumentReason>> SetValueAsync(LocalIri requester, JObject value);
        public Task<Result<DocumentReason>> UpdateValueAsync(JObject value);
        [AlwaysInterleave]
        public Task<Result<JObject, DocumentReason>> GetValueAsync(Iri requester);
        public Task<Result<JArray, DocumentReason>> GetExpandedValueAsync(Iri requester);
    }
}
