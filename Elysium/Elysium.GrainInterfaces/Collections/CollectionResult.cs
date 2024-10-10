using Elysium.Core.Models;

namespace Elysium.GrainInterfaces.Collections
{
    [GenerateSerializer]
    public class CollectionResult
    {
        [Id(0)]
        public required List<Iri> References { get; set; }
        [Id(1)]
        public long Last { get; set; }
    }
}
