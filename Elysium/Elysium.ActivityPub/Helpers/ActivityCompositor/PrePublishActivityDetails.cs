using Newtonsoft.Json.Linq;

namespace Elysium.ActivityPub.Helpers.ActivityCompositor
{
    public class PrePublishActivityDetails
    {
        public required JArray ReferencedActivityWithBtoBcc { get; set; }
        public required JArray ObjectWithBtoBcc { get; set; }
    }
}
