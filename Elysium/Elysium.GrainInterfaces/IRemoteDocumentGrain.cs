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
    public interface IRemoteDocumentGrain : IGrainWithRemoteUriKey
    {
        [AlwaysInterleave]
        public Task<Result<JObject>> GetValueAsync(IHttpMessageAuthor requester, bool skipCachingFirstLayer);
        public Task<Result<JArray>> GetExpandedValueAsync(IHttpMessageAuthor requester, bool skipCachineFirstLayer);
    }
}
