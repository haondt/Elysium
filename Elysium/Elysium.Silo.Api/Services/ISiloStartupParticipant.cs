
namespace Elysium.Silo.Api.Services
{
    public interface ISiloStartupParticipant
    {
        Task OnStartupAsync();
    }
}