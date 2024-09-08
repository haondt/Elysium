using DotNext;
using Elysium.GrainInterfaces.Services;
using Elysium.Hosting.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains.Services
{
    public interface IJsonLdService
    {
        Task<Result<JArray>> ExpandAsync(IHttpMessageAuthor author, JToken input);
        Task<Result<JObject>> CompactAsync(IHttpMessageAuthor author, JArray input);
    }
}
