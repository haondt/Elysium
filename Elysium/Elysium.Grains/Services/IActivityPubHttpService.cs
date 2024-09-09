using Elysium.GrainInterfaces.Reasons;
using Haondt.Core.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains.Services
{
    public interface IActivityPubHttpService
    {
        public Task<Result<ElysiumWebReason>> PostAsync(HttpPostData data);
        public Task<Result<JToken, ElysiumWebReason>> GetAsync(HttpGetData data);

        //public Task<Result<bool>> VerifySignature(idk bro);
    }
}
