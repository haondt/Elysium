using Elysium.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }

    [RegisterConverter]
    public sealed class IriConverter : IConverter<Iri, IriSurrogate>
    {
        public Iri ConvertFromSurrogate(in IriSurrogate surrogate)
        {
            return new Iri(surrogate.Scheme, surrogate.Host, surrogate.Path);
        }

        public IriSurrogate ConvertToSurrogate(in Iri value)
        {
            return new IriSurrogate { Host = value.Host, Path = value.Path, Scheme = value.Scheme };
        }
    }
}
