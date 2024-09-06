using KristofferStrube.ActivityStreams;
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
        public required List<(string, string)> Headers { get; set; } = [];
        public required string CompliantRequestTarget { get; set; }
        public required Uri Target { get; set; }
    }
}
