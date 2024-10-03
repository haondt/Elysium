using Elysium.Core.Models;

namespace Elysium.GrainInterfaces.Surrogates
{
    [GenerateSerializer]
    public struct IriSurrogate
    {
        [Id(0)]
        public string Host;
        [Id(1)]
        public string Path;
        [Id(2)]
        public string Scheme;
        [Id(3)]
        public string? Fragment;
        [Id(4)]
        public string? Query;
    }

    [RegisterConverter]
    public sealed class IriConverter : IConverter<Iri, IriSurrogate>
    {
        public Iri ConvertFromSurrogate(in IriSurrogate surrogate)
        {
            return new Iri(surrogate.Scheme, surrogate.Host, surrogate.Path, surrogate.Fragment, surrogate.Query);
        }

        public IriSurrogate ConvertToSurrogate(in Iri value)
        {
            return new IriSurrogate
            {
                Host = value.Host,
                Path = value.Path,
                Scheme = value.Scheme,
                Fragment = value.Fragment,
                Query = value.Query
            };
        }
    }
}
