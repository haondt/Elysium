using DotNext;
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
        Task<Result<JArray>> ExpandAsync(JToken input);
    }
}
