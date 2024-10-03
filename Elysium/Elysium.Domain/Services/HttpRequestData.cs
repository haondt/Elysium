using Elysium.Core.Models;
using Elysium.GrainInterfaces.Services;

namespace Elysium.Domain.Services
{
    public class HttpRequestData
    {
        public required RemoteIri Target { get; set; }
        public required IHttpMessageAuthor Author { get; set; }

    }
}
