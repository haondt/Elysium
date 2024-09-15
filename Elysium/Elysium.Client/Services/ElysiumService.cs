using Elysium.ActivityPub.Models;
using Elysium.Core.Models;
using Elysium.Cryptography.Services;
using Elysium.GrainInterfaces;
using Elysium.GrainInterfaces.Services;
using Elysium.Hosting.Services;
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
        IIriService iriService,
        IHostingService hostingService,
        IUserCryptoService cryptoService,
        IGrainFactory<LocalIri> grainFactory,
        UserManager<UserIdentity> userManager
        ) : IElysiumService
    {

        public async Task<Result<UserIdentity, List<string>>> RegisterUserAsync(string localizedUsername, string password)
        {
            var (publicKey, encryptedPrivateKey) = cryptoService.GenerateKeyPair();
            var username = iriService.GetActornameFromLocalizedActorname(localizedUsername);
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

            var userIri = iriService.GetIriForLocalizedActorname(localizedUsername);
            try
            {
                var grain = grainFactory.GetGrain<ILocalActorGrain>(userIri);
                await grain.InitializeAsync(new ActorRegistrationDetails
                {
                    EncryptedSigningKey = encryptedPrivateKey,
                    PublicKey = publicKey,
                    Type = JsonLdTypes.PERSON
                });
            }
            catch
            {
                // todo: should probably log this? or notify something? this would be real bad
                // todo: maybe implement some sort of saga pattern here to make recovery more viable
                // todo: saga pattern
                throw;
            }

            return new(user);
        }

        public async Task<Result<Iri, string>> GetIriForFediverseUsernameAsync(string fediverseUsername)
        {
            if (fediverseUsername.Count(c => c == '@') != 1)
                throw new InvalidOperationException("unable to parse username");
            var parts = fediverseUsername.Split('@');
            var partialUri = new IriBuilder { Host = parts[1], Scheme = Uri.UriSchemeHttps }.Iri;
            if (partialUri.Host == hostingService.Host)
            {
                var user = await userManager.FindByNameAsync(parts[0]);
                if (user == null)
                    return new($"Unable to find user {fediverseUsername}");
                var localizedUsername = user.LocalizedUsername ?? user.Id.Parts[^1].Value;
                return new(iriService.GetIriForLocalizedActorname(localizedUsername).Iri);
            }
            return new((await GetUriForRemoteUsernameAsync(fediverseUsername)).Iri);
        }

        public string GetFediverseUsernameAsync(string username, string host)
        {
            return $"{username}@{host}";
        }

        public Task<RemoteIri> GetUriForRemoteUsernameAsync(string username)
        {
            //if (username.Count(c => c == '@') != 1)
            //    return new(new InvalidOperationException("unable to parse username as a remote user"));
            //var pattern = $"^([^@])@(.*)$";
            //var match = Regex.Match(username, pattern);
            //if (!match.Success)
            //    return new(new InvalidOperationException("unable to parse username as a remote user"));
            //try
            //{
            //    return new RemoteUri {  Iri = new Iri($"https://{}")}
            //}

            // TODO: this needs to use webfinger, maybe should be moved to IActivityPubClientService
            //var pattern = $"^([{AuthenticationConstants.ALLOWED_USERNAME_CHARACTERS.Replace("]", @"\]")}]+)@{Regex.Escape(_host)}$";
            //var match = Regex.Match(username, pattern);
            //if (!match.Success)
            //    throw new InvalidOperationException("unable to parse username as a local user");
            //return match.Groups[1].Value;
            throw new NotImplementedException();
        }
    }
}
