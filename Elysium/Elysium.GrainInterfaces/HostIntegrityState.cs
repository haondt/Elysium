using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces
{
    public class HostIntegrityState
    {
        public HostIntegrity Integrity { get; set; } = HostIntegrity.Stable;
        public int NegativeVotes { get; set; }
        public int ConsecutivePositiveVotes { get; set; }
        public DateTime DeactivatedUntil { get; set; }
    }

    public enum HostIntegrity
    {
        Stable,
        Testing,
        Faulty,
        Dead
    }
}
