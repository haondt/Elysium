using Elysium.GrainInterfaces.Services;
using Elysium.Hosting.Models;
using Haondt.Core.Models;
using Haondt.Web.Core.Reasons;
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
    public interface IRemoteDocumentGrain : IGrain<RemoteIri>
    {
        [AlwaysInterleave]
        public Task<Result<JObject, WebReason>> GetValueAsync(IHttpMessageAuthor requester, bool skipCachingFirstLayer);
        public Task<Result<JArray, WebReason>> GetExpandedValueAsync(IHttpMessageAuthor requester, bool skipCachineFirstLayer);
    }
}
