namespace Elysium.Domain.Exceptions
{
    [GenerateSerializer]
    public class ActivityPubException : Exception
    {
        public ActivityPubException() : base() { }
        public ActivityPubException(string message) : base(message) { }

        public ActivityPubException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
