using Haondt.Web.Core.Components;

namespace Elysium.Components.Components
{
    public class TemporaryMessageComponentLayoutModel : IComponentModel
    {
        public required List<TemporaryMessageModel> Messages { get; set; }
    }

    public class TemporaryMessageModel
    {
        public required string Author { get; set; }
        public required string Text { get; set; }
        public required DateTime TimeStamp { get; set; }
    }
}
