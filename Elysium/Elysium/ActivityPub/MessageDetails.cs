namespace Elysium.ActivityPub
{
    public class MessageDetails : IActivityObjectDetails
    {
        public string Type => "https://www.w3.org/ns/activitystreams#Note";
        public required string Text { get; set; }
        public required Uri Recepient { get; set; }

    }
}
