using Elysium.Core.Models;
using Elysium.GrainInterfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces
{
    [GenerateSerializer]
    public class LocalActorState
    {
        public required LocalIri Inbox { get; set; }
    }
}
