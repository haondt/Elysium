using DotNext;
using Newtonsoft.Json.Linq;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces
{
    public interface ILocalDocumentGrain : IGrainWithStringKey
    {
        public Task SetValueAsync(JObject value);
        public Task<Result<JObject>> GetValueAsync();
        public Task<Result<JArray>> GetExpandedValueAsync();
    }
}
