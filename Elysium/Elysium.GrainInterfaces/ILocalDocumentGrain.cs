using Elysium.GrainInterfaces.Reasons;
using Elysium.GrainInterfaces.Services;
using Elysium.Hosting.Models;
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
    public interface ILocalDocumentGrain : IGrain<LocalUri>
    {
        public Task<bool> HasValueAsync(Uri requester);
        public Task InitializeValueAsync(LocalUri owner, JObject value);
        public Task<Result<DocumentReason>> SetValueAsync(LocalUri requester, JObject value);
        public Task<Result<DocumentReason>> UpdateValueAsync(JObject value);
        [AlwaysInterleave]
        public Task<Result<JObject, DocumentReason>> GetValueAsync(Uri requester);
        public Task<Result<JArray, DocumentReason>> GetExpandedValueAsync(Uri requester);
    }
}
