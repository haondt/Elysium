namespace Elysium.ActivityPub
{
    public class MessageDetails : IActivityDetails
    {
        public string Type => "https://www.w3.org/ns/activitystreams#Note";
        public required string Content { get; set; }

    }
}
