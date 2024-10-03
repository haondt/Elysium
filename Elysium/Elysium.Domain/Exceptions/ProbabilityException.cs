namespace Elysium.Domain.Exceptions
{

    [GenerateSerializer]
    public class ProbabilityException : Exception
    {
        public ProbabilityException()
        {
        }

        public ProbabilityException(string? message) : base(message)
        {
        }

        public ProbabilityException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

    }
}
