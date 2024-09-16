using Elysium.Core.Models;
using Elysium.GrainInterfaces.Reasons;
using Haondt.Core.Models;

namespace Elysium.Client.Services
{
    public interface IUserSessionService
    {
        bool IsAuthenticated { get; }

        Optional<LocalIri> GetActiveShade();
        Task<Result<LocalIri, ElysiumWebReason>> GetIriAsync();
        Task<Result<string, ElysiumWebReason>> GetLocalizedUsernameAsync();
        Task<Result<List<LocalIri>, ElysiumWebReason>> GetShadesAsync();
    }
}