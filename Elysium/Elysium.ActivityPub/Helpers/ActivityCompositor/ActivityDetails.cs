using Elysium.Core.Models;

namespace Elysium.ActivityPub.Helpers.ActivityCompositor
{
    public abstract class ActivityDetails : AbstractCompositionDetails
    {
        public required IdentityCompositionDetail Identity { get; set; }
        public required AddressingCompositionDetail Addressing { get; set; }
        public required OwnershipCompositionDetail Ownership { get; set; }

        // activity things
        public required Iri Object { get; set; }
        public required Iri Actor { get; set; }

        protected override List<ICompositionDetail> AggregateDetails()
        {
            return
            [
                Identity,
                Addressing,
                Ownership
            ];
        }

        protected override ActivityPubJsonBuilder AdditionalConfiguration(ActivityPubJsonBuilder builder)
        {
            return builder
                .Actor(Actor)
                .Object(Object);
        }
    }
}
