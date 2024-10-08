﻿using Elysium.ActivityPub.Models;

namespace Elysium.ActivityPub.Helpers.ActivityCompositor
{
    public class MessageDetails : AbstractCompositionDetails, IActivityObjectDetails
    {
        public override string Type => JsonLdTypes.NOTE;
        public required OwnershipCompositionDetail Ownership { get; set; }
        public required AddressingCompositionDetail Addressing { get; set; }

        public required string Text { get; set; }
        protected override List<ICompositionDetail> AggregateDetails()
        {
            return
            [
                Addressing,
                Ownership
            ];
        }

        protected override ActivityPubJsonBuilder AdditionalConfiguration(ActivityPubJsonBuilder builder)
        {
            return builder
                .Content(Text);
        }
    }
}
