using Elysium.Hosting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces
{
    public class OutgoingRemoteActivityData
    {
        public required string Payload { get; set; }
        public required LocalUri Sender { get; set; }
    }
}
