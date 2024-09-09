using Elysium.GrainInterfaces.Reasons;
using Elysium.GrainInterfaces.Services;
using Elysium.Hosting.Models;
using Haondt.Core.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains.Services
{
    public interface IDocumentResolver
    {
        Task<Result<JToken, ElysiumWebReason>> GetDocumentAsync(IHttpMessageAuthor requester, RemoteUri uri);
        Task<Result<JToken, ElysiumWebReason>> GetDocumentAsync(IHttpMessageAuthor requester, LocalUri uri);
        //Task<Result<JObject>> GetDocumentAsync(RemoteUri requester, LocalUri uri);
    }
}
