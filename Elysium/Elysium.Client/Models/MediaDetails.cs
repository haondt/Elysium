namespace Elysium.Client.Models
{
    public class MediaDetails
    {
        public int Depth { get; set; }
        public string? AuthorName { get; set; }
        public string? AuthorFediverseHandle { get; set; }
        public string? Timestamp { get; set; }
        public string? Title { get; set; }
        public string? Text { get; set; }
        public int NumReplies { get; set; }
    }
}
