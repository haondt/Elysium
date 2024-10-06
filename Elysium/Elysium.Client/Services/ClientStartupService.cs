namespace Elysium.Client.Services
{
    public class ClientStartupService(IEnumerable<IClientStartupParticipant> participants) : IClientStartupService
    {
        public async Task OnStartupAsync()
        {
            foreach (var participant in participants)
                await participant.OnStartupAsync();
        }
    }
}
