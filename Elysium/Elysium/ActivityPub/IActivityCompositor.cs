using Newtonsoft.Json.Linq;

namespace Elysium.ActivityPub
{
    public interface IActivityCompositor
    {
        public JObject Composit();
    }
}
