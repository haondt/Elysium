using DotNext;
using Elysium.GrainInterfaces.Services;
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
        Task<Result<JObject>> GetDocumentAsync(IHttpMessageAuthor requester, RemoteUri uri);
        Task<Result<JObject>> GetDocumentAsync(Uri requester, LocalUri uri);
    }
}
