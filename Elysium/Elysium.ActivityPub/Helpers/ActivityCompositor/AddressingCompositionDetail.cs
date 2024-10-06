using Elysium.ActivityPub.Models;
using Elysium.Core.Models;

namespace Elysium.ActivityPub.Helpers.ActivityCompositor
{
    public class AddressingCompositionDetail : ICompositionDetail
    {
        public List<Iri>? Cc { get; set; }
        public List<Iri>? To { get; set; }
        public List<Iri>? Bto { get; set; }
        public List<Iri>? Bcc { get; set; }

        public ActivityPubJsonBuilder Apply(ActivityPubJsonBuilder builder)
        {
            return builder
                .To(To)
                .Bto(Bto)
                .Cc(Cc)
                .Bcc(Bto);
        }

        // todo: add an argument for To, as usually you will want to send your public post to your followers
        public static AddressingCompositionDetail Public { get; } = new()
        {
            Cc = [ActivityPubConstants.PUBLIC_COLLECTION]
        };
    }
}
