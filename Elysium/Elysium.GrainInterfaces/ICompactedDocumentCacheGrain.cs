using Elysium.Core.Models;
using Haondt.Core.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces
{
    public interface ICompactedDocumentCacheGrain : IGrain<Iri>
    {
        Task<Optional<DocumentState<JObject>>> TryGetValueAsync();
        Task SetValueAsync(DocumentState<JObject> value);
        Task ClearAsync();
    }
}
