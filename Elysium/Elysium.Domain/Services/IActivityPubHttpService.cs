using Elysium.GrainInterfaces.Reasons;
using Haondt.Core.Models;
using Newtonsoft.Json.Linq;

namespace Elysium.Domain.Services
{
    public interface IActivityPubHttpService
    {
        public Task<Result<ElysiumWebReason>> PostAsync(HttpPostData data);
        public Task<Result<JToken, ElysiumWebReason>> GetAsync(HttpGetData data);

        //public Task<Result<bool>> VerifySignature(idk bro);
    }
}
