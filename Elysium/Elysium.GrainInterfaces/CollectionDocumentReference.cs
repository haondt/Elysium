using Elysium.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces
{
    public class CollectionDocumentReference
    {
        public required string  Iri { get; set; }
        public required int Previous { get; set; }
    }
}
