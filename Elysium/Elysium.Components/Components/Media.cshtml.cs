using Haondt.Web.Core.Components;

namespace Elysium.Components.Components
{
    public class MediaModel : IComponentModel
    {
        public required int Depth { get; set; }
        public required string AuthorName { get; set; }
        public required string AuthorFediverseHandle { get; set; }
        public required string Timestamp { get; set; }
        public string? SanitizedTitle { get; set; }
        public string? SanitizedText { get; set; }
    }
}
