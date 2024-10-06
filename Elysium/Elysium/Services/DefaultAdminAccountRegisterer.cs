using Elysium.Client.Services;
using Elysium.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Elysium.Services
{
    public class DefaultAdminAccountRegisterer(
        IOptions<AdminSettings> options,
        UserManager<UserIdentity> userManager,
        IElysiumService elysiumService
        ) : IClientStartupParticipant
    {
        public async Task OnStartupAsync()
        {
            if (!options.Value.RegisterDefaultAdminUser)
                return;

            if (string.IsNullOrWhiteSpace(options.Value.DefaultAdminUsername))
                throw new ArgumentException("Default admin username cannot be empty.");
            if (string.IsNullOrWhiteSpace(options.Value.DefaultAdminPassword))
                throw new ArgumentException("Default admin password cannot be empty.");

            if (await userManager.FindByNameAsync(options.Value.DefaultAdminUsername) != null)
                return;

            var result = await elysiumService.RegisterAdministratorAsync(options.Value.DefaultAdminUsername, options.Value.DefaultAdminPassword);
            if (!result.IsSuccessful)
                throw new InvalidOperationException($"Unable to register default admin due to error(s): {string.Join('\n', result.Reason)}");
        }
    }
}
