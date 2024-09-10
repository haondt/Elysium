using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Core.Models
{
    [GenerateSerializer, Immutable]
    public readonly record struct RemoteIri(Iri iri)
    {
        public Iri Iri { get; init; } = iri;
    }
}

