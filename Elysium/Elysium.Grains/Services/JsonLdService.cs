using DotNext;
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
        public async Task<Result<JObject>> CompactAsync(IHttpMessageAuthor author, JArray input)
        {
            try
            {
                return await JsonLdProcessor.CompactAsync(input, null, new JsonLdOptions(string.Empty)
                {
                    documentLoader = new ElysiumDocumentLoader(documentResolver, author, hostingService),
                });
            }
            catch (Exception ex)
            {
                return new(ex);
            }
        }

        public Task<Result<JObject>> CompactAsync(RemoteUri author, JArray input)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<JArray>> ExpandAsync(IHttpMessageAuthor author, JToken input)
        {
            try
            {
                return await JsonLdProcessor.ExpandAsync(input, new JsonLdOptions(string.Empty)
                {
                    documentLoader = new ElysiumDocumentLoader(documentResolver, author, hostingService),
                });
            }
            catch (Exception ex)
            {
                return new(ex);
            }
        }

        public Task<Result<JArray>> ExpandAsync(RemoteUri author, JToken input)
        {
            throw new NotImplementedException();
        }
    }
}
