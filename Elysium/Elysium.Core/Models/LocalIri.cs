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

        public static implicit operator Iri(LocalIri localIri) => localIri.Iri;
    }
}

