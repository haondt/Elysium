using Elysium.ActivityPub.Helpers.ActivityCompositor;
using Elysium.ActivityPub.Models;

namespace Elysium.ActivityPub.Helpers.ActivityCompositor
{
    public class MessageDetails : IActivityObjectDetails
    {
        public string Type => JsonLdTypes.NOTE;
        public required string Text { get; set; }
        public required Uri Recepient { get; set; }
    }
}
