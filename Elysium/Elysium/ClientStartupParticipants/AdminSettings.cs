namespace Elysium.ClientStartupParticipants
{
    public class AdminSettings
    {
        public bool RegisterDefaultAdminUser { get; set; } = false;
        public string? DefaultAdminUsername { get; set; }
        public string? DefaultAdminPassword { get; set; }

    }
}
