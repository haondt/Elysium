using Elysium.Core.Models;

namespace Elysium.Core.Extensions
{
    public static class IriExtensions
    {
        public static LocalIri Concatenate(this LocalIri iri, string subpath) => new() { Iri = iri.Iri.Concatenate(subpath) };
    }
}
