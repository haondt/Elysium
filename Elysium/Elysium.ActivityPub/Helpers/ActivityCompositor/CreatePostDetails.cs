using Elysium.ActivityPub.Models;

namespace Elysium.ActivityPub.Helpers.ActivityCompositor
{
    public class CreatePostDetails : AbstractCompositionDetails
    {
        public override string Type => JsonLdTypes.NOTE;
        public required OwnershipCompositionDetail Ownership { get; set; }
        public required AddressingCompositionDetail Addressing { get; set; }

        public string? Title { get; set; }
        public string? Text { get; set; }

        protected override List<ICompositionDetail> AggregateDetails()
        {
            return [
                Ownership,
                Addressing
            ];
        }

        protected override ActivityPubJsonBuilder AdditionalConfiguration(ActivityPubJsonBuilder builder)
        {
            if (Title != null)
                builder.Name(Title);
            if (Text != null)
                builder.Content(Text);
            return builder;
        }
    }
}
