using Elysium.ActivityPub.Models;
using Elysium.Core.Models;

namespace Elysium.ActivityPub.Helpers.ActivityCompositor
{
    public class MessageDetails : IActivityObjectDetails
    {
        public string Type => JsonLdTypes.NOTE;
        public required string Text { get; set; }
        public required Iri Recepient { get; set; }
        public required Iri AttributedTo { get; set; }
    }
}
