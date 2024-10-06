namespace Elysium.Client.Services
{
    public interface IClientStartupParticipant
    {
        public Task OnStartupAsync();
    }
}
