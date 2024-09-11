using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces.Services
{
    [GenerateSerializer, Immutable]
    public class ActorRegistrationDetails
    {
        [Id(0)]
        public required string EncryptedSigningKey { get; set; }
        [Id(1)]
        public required string PublicKey { get; set; }
        [Id(2)]
        public required string Type { get; set; }
    }
}
