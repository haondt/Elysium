using Elysium.Core.Models;
using Elysium.GrainInterfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces
{
    [GenerateSerializer, Immutable]
    public class DispatchRemoteActivityData 
    {
        public required string Payload { get; set; }
        public required LocalIri Sender { get; set; }
        public required RemoteIri Target { get; set; }
    }
}
