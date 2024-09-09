using Elysium.Authentication.Services;
using Elysium.Core.Models;
using Elysium.GrainInterfaces;
using Elysium.Server.Services;
using Haondt.Core.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Client.Services
{
    public class ElysiumService(
        IHostingService hostingService,
        IUserCryptoService cryptoService,
        IGrainFactory grainFactory,
        UserManager<UserIdentity> userManager
        ) : IElysiumService
    {
        private readonly ILocalActorRegistryGrain _registryGrain = grainFactory.GetGrain<ILocalActorRegistryGrain>(Guid.Empty);

        public async Task<Result<UserIdentity, List<string>>> RegisterUserAsync(string localizedUsername, string password)
        {
            var (publicKey, encryptedPrivateKey) = cryptoService.GenerateKeyPair();
            var username = hostingService.GetUsernameFromLocalizedUsername(localizedUsername);
            var user = new UserIdentity
            {
                Id = UserIdentity.GetStorageKey(username),
                LocalizedUsername = localizedUsername,
                PublicKey = publicKey,
                EncryptedPrivateKey = encryptedPrivateKey
            };
            var result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
                return new(result.Errors.Select(e => $"{e.Description}").ToList());

            var userUri = hostingService.GetUriForLocalizedUsername(localizedUsername);
            throw new NotImplementedException();
            await _registryGrain.RegisterActor(userUri, new LocalActorState
            {
                Inbox = new () // TODO!
            });
            return new(user);
        }
    }
}
