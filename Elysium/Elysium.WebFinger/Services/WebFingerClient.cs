using DotNext;

namespace Elysium.WebFinger.Services
{
    public class WebFingerClient(HttpClient httpClient)
    {
        public Result<JsonResourceDescriptor> GetAsync(WebFingerQuery query)
        {
            // TODO

        }
    }
}
