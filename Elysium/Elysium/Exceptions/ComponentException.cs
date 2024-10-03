namespace Elysium.Exceptions
{
    public class ComponentException : Exception
    {
        public ComponentException()
        {
        }

        public ComponentException(string? message) : base(message)
        {
        }

        public ComponentException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
