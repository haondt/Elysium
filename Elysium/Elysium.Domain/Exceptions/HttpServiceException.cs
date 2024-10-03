using Elysium.Core.Models;
using Elysium.GrainInterfaces.Services;

namespace Elysium.Domain.Exceptions
{
    [GenerateSerializer]
    public class HttpServiceException : Exception
    {
        private static string DefaultMessageFactory(RemoteIri target, IHttpMessageAuthor author)
        {
            return $"Failed to send requests to {target}";

        }
        public HttpServiceException(RemoteIri target, IHttpMessageAuthor author) : base(DefaultMessageFactory(target, author))
        {
        }

        public HttpServiceException(RemoteIri target, IHttpMessageAuthor author, string? message) : base(message ?? DefaultMessageFactory(target, author))
        {
        }

        public HttpServiceException(RemoteIri target, IHttpMessageAuthor author, string? message, Exception? innerException) : base(message ?? DefaultMessageFactory(target, author), innerException)
        {
        }
    }
}
