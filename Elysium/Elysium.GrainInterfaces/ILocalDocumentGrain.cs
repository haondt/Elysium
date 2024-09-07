using DotNext;
using Elysium.GrainInterfaces.Services;
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
    public interface ILocalDocumentGrain : IGrainWithLocalUriKey
    {
        public Task<bool> HasValueAsync(Uri requester);
        public Task<Optional<Exception>> SetValueAsync(LocalUri actor, JObject value);
        public Task<Optional<Exception>> UpdateValueAsync(JObject value);
        [AlwaysInterleave]
        public Task<Result<JObject>> GetValueAsync(Uri requester);
        public Task<Result<JArray>> GetExpandedValueAsync(Uri requester);
    }
}
