using Elysium.ActivityPub.Models;
using Elysium.Core.Extensions;
using Elysium.GrainInterfaces.Services;
using Elysium.Hosting.Services;
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
    public class JsonLdService(IDocumentService documentResolver, IHostingService hostingService) : IJsonLdService
    {
        public async Task<JObject> CompactAsync(IHttpMessageAuthor author, JToken input)
        {
            var flattened = (await JsonLdProcessor.FlattenAsync(input, new JObject(), new JsonLdOptions(null)
            {
                documentLoader = new ElysiumDocumentLoader(documentResolver, author, hostingService),
            })).As<JObject>();

            var contexts = new HashSet<string>();


            if (flattened.TryGetValue("@context", out var flattenedContext))
            {
                if (flattenedContext.Is<JArray>(out var ja))
                    contexts = ja.Select(v => v.AsString()).ToHashSet();
                if (flattenedContext.Is<JValue>(out var jv))
                    contexts.Add(jv.AsString());
            }

            foreach(var node in flattened.Get<JArray>("@graph"))
                foreach(var prop in node.As<JObject>().Properties())
                {
                    if (JsonLdContextMappings.TryGetContext(prop.Name, out var propContext))
                        contexts.Add(propContext);
                    if (prop.Name == "@type")
                        if (JsonLdContextMappings.TryGetContext(prop.Value.AsString(), out var typeContext))
                            contexts.Add(typeContext);
                }



            var context = contexts.Count > 0
                ? new JObject { { "@context", new JArray(contexts) } }
                : [];
            //var context = contexts.Count > 0
            //    ?  new JArray(contexts)
            //    : [];

            return await JsonLdProcessor.CompactAsync(input, context, new JsonLdOptions(null)
            {
                documentLoader = new ElysiumDocumentLoader(documentResolver, author, hostingService),
            });
        }

        public async Task<JArray> ExpandAsync(IHttpMessageAuthor author, JToken input)
        {
            return await JsonLdProcessor.ExpandAsync(input, new JsonLdOptions(null)
            {
                documentLoader = new ElysiumDocumentLoader(documentResolver, author, hostingService),
            });
        }
    }
}
