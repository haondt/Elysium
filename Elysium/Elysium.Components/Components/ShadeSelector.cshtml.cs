using Haondt.Web.Core.Components;

namespace Elysium.Components.Components
{
    public class ShadeSelectorModel : IComponentModel
    {
        public required List<ShadeSelection> ShadeSelections { get; set; }
    }

    public class ShadeSelection
    {
        public bool IsActive { get; set; }
        public bool IsPrimary { get; set; }
        public bool HasNotifications { get; set; }
        public required string Text { get; set; }
    }
}
