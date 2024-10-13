
namespace Elysium.Silo.Api.Services
{
    public class SiloStartupService(IEnumerable<ISiloStartupParticipant> participants) : IStartupTask
    {
        public async Task Execute(CancellationToken cancellationToken)
        {
            foreach (var participant in participants)
                await participant.OnStartupAsync();
        }
    }
}

