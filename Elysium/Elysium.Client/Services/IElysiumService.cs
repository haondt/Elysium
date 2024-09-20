using Elysium.Components.Components;
using Elysium.Core.Models;
using Haondt.Core.Models;

namespace Elysium.Client.Services
{
    public interface IElysiumService
    {
        string GetFediverseUsername(string username, string host);
        Task<Result<Iri, string>> GetIriForFediverseUsernameAsync(string fediverseUsername);
        Task<(IEnumerable<MediaModel> Creations, string Last)> GetPublicCreations(string? before = null);
        string GetShadeNameFromLocalIri(LocalIri userIri, LocalIri shadeIri);
        Task<Result<UserIdentity, List<string>>> RegisterUserAsync(string localizedUsername, string password);
    }
}