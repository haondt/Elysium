
using Elysium.Core.Models;
using Newtonsoft.Json.Linq;

namespace Elysium.Silo.Api.Services
{
    public interface IDevHandler
    {
        Task<(LocalIri ActivityIri, JObject Activity)> CreateForLocal(DevLocalActivityPayload payload);
    }
}