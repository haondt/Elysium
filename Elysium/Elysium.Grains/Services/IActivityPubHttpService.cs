using DotNext;
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
        public Task<Optional<Exception>> PostAsync(HttpPostData data);
        public Task<Result<JObject>> GetAsync(HttpGetData data);

        //public Task<Result<bool>> VerifySignature(idk bro);
    }
}
