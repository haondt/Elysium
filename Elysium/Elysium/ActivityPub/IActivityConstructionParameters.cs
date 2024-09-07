namespace Elysium.ActivityPub
{
    public interface IActivityObjectDetails
    {
        public string Type { get; }
        public Uri Recepient { get; }
    }
}
