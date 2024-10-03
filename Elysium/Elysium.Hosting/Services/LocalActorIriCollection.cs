using Elysium.Core.Models;

namespace Elysium.Hosting.Services
{
    public class LocalActorIriCollection
    {
        public required LocalIri Inbox { get; init; }
        public required LocalIri Outbox { get; init; }
        public required LocalIri Following { get; init; }
        public required LocalIri Followers { get; init; }
        public required LocalIri PublicKey { get; init; }

    }
}
