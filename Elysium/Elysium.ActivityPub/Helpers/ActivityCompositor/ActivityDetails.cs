using Elysium.Core.Models;

namespace Elysium.ActivityPub.Helpers.ActivityCompositor
{
    public abstract class ActivityDetails : ICompositionDetails
    {
        public required Iri Id { get; set; }
        public abstract string Type { get; }
        public required Iri Actor { get; set; }
        public List<Iri>? Cc { get; set; }
        public List<Iri>? To { get; set; }
        public List<Iri>? Bto { get; set; }
        public List<Iri>? Bcc { get; set; }
        public required Iri AttributedTo { get; set; }
        public required Iri Object { get; set; }
    }
}
