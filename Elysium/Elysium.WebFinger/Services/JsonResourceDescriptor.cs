namespace Elysium.WebFinger.Services
{
    public class JsonResourceDescriptor
    {
        public required string Subject { get; set; }
        public List<string> Aliases { get; set; } = [];
        public Dictionary<string, string> Properties { get; set; } = [];
        public List<JsonResourceDescriptorLink> Links { get; set; } = [];
    }

    public class JsonResourceDescriptorLink
    {
        public required string Rel { get; set; }
        public required string Href { get; set; }
    }
}
