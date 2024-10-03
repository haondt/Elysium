using Elysium.GrainInterfaces.Services;
using Newtonsoft.Json.Linq;

namespace Elysium.Domain.Services
{
    public interface IJsonLdService
    {
        Task<JArray> ExpandAsync(IHttpMessageAuthor author, JToken input);
        Task<JObject> CompactAsync(IHttpMessageAuthor author, JToken input);
    }
}
