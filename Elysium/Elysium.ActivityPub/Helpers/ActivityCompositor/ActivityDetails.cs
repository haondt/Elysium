namespace Elysium.ActivityPub.Helpers.ActivityCompositor
{
    public abstract class ActivityDetails : ICompositionDetails
    {
        public abstract string Type { get; }
        public required Uri Actor { get; set; }
        public List<Uri>? Cc { get; set; }
        public List<Uri>? To { get; set; }
        public List<Uri>? Bto { get; set; }
        public List<Uri>? Bcc { get; set; }
        public required Uri AttributedTo { get; set; }
        public required Uri Object { get; set; }
    }
}
