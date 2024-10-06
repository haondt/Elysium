namespace Elysium.Authentication.Exceptions
{
    public class NeedsAuthorizationException : Exception
    {
        public NeedsAuthorizationException()
        {
        }

        public NeedsAuthorizationException(string? message) : base(message)
        {
        }

        public NeedsAuthorizationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
