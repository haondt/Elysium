using Newtonsoft.Json.Linq;

namespace Elysium.Silo.Api.Services
{
    public class DevLocalActivityPayload
    {
        public string? NewActorType { get; set; }
        public required string  ActorName { get; set; }
        public JObject? SubjectObject { get; set; }
        public string? SubjectLink { get; set; }
    }
}
