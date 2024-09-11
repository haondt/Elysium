using Elysium.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Core.Extensions
{
    public static class IriExtensions
    {
        public static LocalIri Concatenate(this LocalIri iri, string subpath) => new() { Iri = iri.Iri.Concatenate(subpath) };
    }
}
