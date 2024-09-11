using Elysium.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
