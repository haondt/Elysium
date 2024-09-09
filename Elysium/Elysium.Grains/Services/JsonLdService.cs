using Elysium.GrainInterfaces.Services;
using Elysium.Hosting.Models;
using Elysium.Server.Services;
using JsonLD.Core;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains.Services
{
    public class JsonLdService(IDocumentResolver documentResolver, IHostingService hostingService) : IJsonLdService
    {
        public async Task<JObject> CompactAsync(IHttpMessageAuthor author, JArray input)
        {
            return await JsonLdProcessor.CompactAsync(input, null, new JsonLdOptions(string.Empty)
            {
                documentLoader = new ElysiumDocumentLoader(documentResolver, author, hostingService),
            });
        }

        public async Task<JArray> ExpandAsync(IHttpMessageAuthor author, JToken input)
        {
            return await JsonLdProcessor.ExpandAsync(input, new JsonLdOptions(string.Empty)
            {
                documentLoader = new ElysiumDocumentLoader(documentResolver, author, hostingService),
            });
        }
    }
}
