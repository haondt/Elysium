using DotNext;
using JsonLD.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains.Services
{
    public class JsonLdService(IDocumentResolver documentResolver) : IJsonLdService
    {
        public async Task<Result<JArray>> ExpandAsync(JToken input)
        {
            try
            {
                return await JsonLdProcessor.ExpandAsync(input, new JsonLdOptions(string.Empty)
                {
                    documentLoader = new ElysiumDocumentLoader(documentResolver),
                });
            }
            catch (Exception ex)
            {
                return new(ex);
            }
        }
    }
}
