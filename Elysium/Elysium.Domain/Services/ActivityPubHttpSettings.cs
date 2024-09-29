namespace Elysium.Domain.Services
{
    public class ActivityPubHttpSettings
    {
        public bool VerifySignatures { get; set; }
        public bool SignFetches { get; set; }
        public bool SignPushes { get; set; }
        public int MaxRedirects { get; set; } = 3;
    }
}