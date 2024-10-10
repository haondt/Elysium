namespace Elysium.GrainInterfaces.Hosting
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
