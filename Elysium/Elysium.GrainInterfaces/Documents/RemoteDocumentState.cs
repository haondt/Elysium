using Elysium.Core.Models;
using Newtonsoft.Json.Linq;

namespace Elysium.GrainInterfaces.Documents
{
    [GenerateSerializer]
    public class RemoteDocumentState
    {

        [Id(0)]
        public JToken? Value { get; set; }
        [Id(1)]
        public Iri? Owner { get; set; }
        [Id(2)]
        public DateTime? UpdatedOnUtc { get; set; }
    }
}
