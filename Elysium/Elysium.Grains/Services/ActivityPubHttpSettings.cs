namespace Elysium.Grains.Services
{
    public class ActivityPubHttpSettings
    {
        public bool VerifySignatures { get; set; }
        public bool SignFetches { get; set; }
        public bool SignPushes { get; set; }
    }
}