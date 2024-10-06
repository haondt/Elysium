using Elysium.Authentication.Constants;
using Elysium.Client.Services;
using Elysium.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace Elysium.Services
{
    public class RoleRegisterer(RoleManager<RoleIdentity> roleManager) : IClientStartupParticipant
    {
        public async Task OnStartupAsync()
        {
            foreach (var role in new List<string>
            {
                AuthenticationConstants.ADMINISTRATOR_ROLE,
                AuthenticationConstants.OWNER_ROLE,
            })
            {
                if (await roleManager.RoleExistsAsync(AuthenticationConstants.ADMINISTRATOR_ROLE))
                    continue;

                var result = await roleManager.CreateAsync(new RoleIdentity
                {
                    Id = RoleIdentity.GetStorageKey(role),
                    Name = role
                });

                if (!result.Succeeded)
                    throw new InvalidOperationException($"Unable to register role \"{role}\" due to error(s): {string.Join('\n', result.Errors)}");
            }
        }
    }
}
