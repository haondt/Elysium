using Haondt.Core.Models;
using Haondt.Web.Core.Components;

namespace Elysium.Components.Components
{
    public class ErrorModel : IComponentModel
    {
        public required int ErrorCode { get; set; }
        public required string Message { get; set; }
        public Optional<string> Title { get; set; }
        public Optional<string> Details { get; set; }
    }
}
