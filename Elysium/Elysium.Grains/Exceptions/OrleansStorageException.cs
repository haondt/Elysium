namespace Elysium.Domain.Exceptions
{
    public class OrleansStorageException : Exception
    {
        public OrleansStorageException()
        {
        }

        public OrleansStorageException(string? message) : base(message)
        {
        }

        public OrleansStorageException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

    }
}
