namespace Elysium.Domain
{
    public class HostIntegritySettings
    {
        public int MaxFailures { get; set; } = 10;
        public int MinPasses { get; set; } = 10;
        public int MaxFaultyFailures { get; set; } = 10;
        public int MinFaultyPasses { get; set; } = 1;
        public int FaultyServerPeriodInHours { get; set; } = 168; // 7 days
    }
}
