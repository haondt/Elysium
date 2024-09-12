using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Core.Models
{
    [GenerateSerializer, Immutable]
    public readonly record struct LocalIri
    {
        [Id(0)]
        public Iri Iri { get; init; }

        public override string ToString()
        {
            return Iri.ToString();
        }
    }
}

