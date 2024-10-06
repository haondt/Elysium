namespace Elysium.Authentication.Exceptions
{
    public class NeedsAuthenticationException : Exception
    {
        public NeedsAuthenticationException()
        {
        }

        public NeedsAuthenticationException(string? message) : base(message)
        {
        }

        public NeedsAuthenticationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

    }
}
