using Elysium.Core.Models;

namespace Elysium.ActivityPub.Helpers.ActivityCompositor
{
    public class IdentityCompositionDetail : ICompositionDetail
    {
        public required Iri Id { get; set; }

        public ActivityPubJsonBuilder Apply(ActivityPubJsonBuilder builder) => builder.Id(Id);
    }
}
