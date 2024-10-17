namespace Elysium.Grains.Queueing.Redis
{
    public class RedisQueueSettings
    {
        public string ChannelDiscriminator { get; set; } = "";
        public int Database { get; set; } = 1;
        public bool Enabled { get; set; } = false;
    }
}