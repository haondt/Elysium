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
    public interface IDocumentService
    {
        Task<Result<JToken, ElysiumWebReason>> GetDocumentAsync(IHttpMessageAuthor requester, RemoteIri uri);
        Task<Result<JToken, ElysiumWebReason>> GetDocumentAsync(IHttpMessageAuthor requester, LocalIri uri);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="objectUri"></param>
        /// <param name="compactedObject"></param>
        /// <param name="bto">may be an empty list</param>
        /// <param name="bcc">may be an empty list</param>
        /// <returns></returns>
        Task<Result<DocumentReason>> CreateDocumentAsync(
            LocalIri actor, 
            LocalIri objectUri, 
            JObject compactedObject,
            List<Uri> bto,
            List<Uri> bcc);
    }
}
