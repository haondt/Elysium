namespace Elysium.Core.Models
{
    [GenerateSerializer, Immutable]
    public readonly record struct RemoteIri(Iri iri)
    {
        [Id(0)]
        public Iri Iri { get; init; } = iri;
        public override string ToString()
        {
            return Iri.ToString();
        }
    }
}

