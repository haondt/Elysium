using Elysium.Client.Models;
using Elysium.Core.Models;
using Haondt.Core.Models;

namespace Elysium.Client.Services
{
    public interface IElysiumService
    {
        Task<string> GenerateInviteLinkAsync();
        string GetFediverseUsername(string username, string host);
        Task<Result<Iri, string>> GetIriForFediverseUsernameAsync(string fediverseUsername);
        Task<(IEnumerable<MediaDetails> Creations, string Last)> GetPublicCreations(string? before = null);
        string GetShadeNameFromLocalIri(LocalIri userIri, LocalIri shadeIri);
        Task<Result<UserIdentity, List<string>>> RegisterAdministratorAsync(string localizedUsername, string password);
        Task<Result<UserIdentity, List<string>>> RegisterUserAsync(string localizedUsername, string password);
    }
}