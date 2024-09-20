using Elysium.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces
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
