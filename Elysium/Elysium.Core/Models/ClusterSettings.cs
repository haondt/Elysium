namespace Elysium.Core.Models
{
    public class ClusterSettings
    {
        public string ClusterId { get; set; } = "default";
        public string ServiceId { get; set; } = "default";
        public ClusteringStrategy ClusteringStrategy { get; set; } = ClusteringStrategy.Localhost;

        public int RedisDatabase { get; set; } = 0;
    }
}
