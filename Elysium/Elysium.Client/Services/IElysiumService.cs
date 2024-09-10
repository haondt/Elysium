using Elysium.Core.Models;
using Haondt.Core.Models;

namespace Elysium.Client.Services
{
    public interface IElysiumService
    {
        Task<Result<Iri, string>> GetIriForFediverseUsernameAsync(string fediverseUsername);
        Task<Result<UserIdentity, List<string>>> RegisterUserAsync(string localizedUsername, string password);
    }
}