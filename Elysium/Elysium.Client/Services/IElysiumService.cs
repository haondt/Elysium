using Elysium.Core.Models;
using Haondt.Core.Models;

namespace Elysium.Client.Services
{
    public interface IElysiumService
    {
        string GetFediverseUsernameAsync(string username, string host);
        Task<Result<Iri, string>> GetIriForFediverseUsernameAsync(string fediverseUsername);
        string GetShadeNameFromLocalIri(LocalIri userIri, LocalIri shadeIri);
        Task<Result<UserIdentity, List<string>>> RegisterUserAsync(string localizedUsername, string password);
    }
}