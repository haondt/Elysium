using Elysium.Core.Models;
using Haondt.Core.Models;

namespace Elysium.ActivityPub.Helpers.ActivityCompositor
{
    public class OwnershipCompositionDetail : ICompositionDetail
    {
        public Optional<Iri> AttributedTo { get; set; } = new();

        public ActivityPubJsonBuilder Apply(ActivityPubJsonBuilder builder)
        {
            if (AttributedTo.HasValue)
                builder = builder.AttributedTo(AttributedTo.Value);
            return builder;
        }

        public static OwnershipCompositionDetail Nobody { get; } = new OwnershipCompositionDetail();

    }
}
