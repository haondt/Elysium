using Elysium.GrainInterfaces.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Domain.Services
{
    public interface IJsonLdService
    {
        Task<JArray> ExpandAsync(IHttpMessageAuthor author, JToken input);
        Task<JObject> CompactAsync(IHttpMessageAuthor author, JToken input);
    }
}
