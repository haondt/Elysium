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
    public interface IExpandedDocumentCacheGrain : IGrain<Iri>
    {
        Task<Optional<DocumentState<JArray>>> TryGetValueAsync();
        Task SetValueAsync(DocumentState<JArray> value);
        Task ClearAsync();
    }
}
