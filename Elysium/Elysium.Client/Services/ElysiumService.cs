using Elysium.ActivityPub.Helpers;
using Elysium.ActivityPub.Models;
using Elysium.Authentication.Constants;
using Elysium.Components.Components;
using Elysium.Core.Converters;
using Elysium.Core.Extensions;
using Elysium.Core.Models;
using Elysium.Cryptography.Services;
using Elysium.Domain.Services;
using Elysium.GrainInterfaces;
using Elysium.GrainInterfaces.Services;
using Elysium.Hosting.Services;
using Haondt.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace Elysium.Client.Services
{
    public class ElysiumService(
        IIriService iriService,
        IHostingService hostingService,
        IUserCryptoService cryptoService,
        IGrainFactory<LocalIri> grainFactory,
        IGrainFactory baseGrainFactory,
        IDocumentService documentService,
        UserManager<UserIdentity> userManager
        ) : IElysiumService
    {
        IPublicCollectionGrain _publicCollectionGrain = baseGrainFactory.GetGrain<IPublicCollectionGrain>(Guid.Empty);
        IInstanceActorAuthorGrain _instanceAuthor = baseGrainFactory.GetGrain<IInstanceActorAuthorGrain>(Guid.Empty);

        public async Task<Result<UserIdentity, List<string>>> RegisterAdministratorAsync(string localizedUsername, string password)
        {
            var user = await RegisterUserAsync(localizedUsername, password);
            if (!user.IsSuccessful)
                return user;

            var result = await userManager.AddToRoleAsync(user.Value, AuthenticationConstants.ADMINISTRATOR_ROLE);
            if (!result.Succeeded)
                return new(result.Errors.Select(e => $"{e.Description}").ToList());

            // todo: saga pattern here too

            return new(user.Value);
        }

        public async Task<Result<UserIdentity, List<string>>> RegisterUserAsync(string localizedUsername, string password)
        {
            var username = iriService.GetActornameFromLocalizedActorname(localizedUsername);
            var user = new UserIdentity
            {
                Id = UserIdentity.GetStorageKey(username),
                LocalizedUsername = localizedUsername,
            };
            var result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
                return new(result.Errors.Select(e => $"{e.Description}").ToList());

            var userIri = iriService.GetIriForLocalizedActorname(localizedUsername);
            var (publicKey, privateKey) = cryptoService.GenerateKeyPair();
            var cryptographicUserData = cryptoService.EncryptCryptographicActorData(new PlaintextCryptographicActorData
            {
                SigningKey = privateKey

            }, userIri);
            try
            {
                var grain = grainFactory.GetGrain<ILocalActorGrain>(userIri);
                await grain.InitializeAsync(new ActorRegistrationDetails
                {
                    PrivateKey = privateKey,
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
                return new(iriService.GetIriForLocalizedActorname(localizedUsername));
            }
            return new((await GetUriForRemoteUsernameAsync(fediverseUsername)));
        }

        public string GetFediverseUsername(string username, string host)
        {
            return $"{username}@{host}";
        }

        public string GetShadeNameFromLocalIri(LocalIri userIri, LocalIri shadeIri)
        {
            var userIriString = userIri.ToString();
            var shadeIriString = shadeIri.ToString();
            if (!shadeIriString.StartsWith(userIriString))
                throw new ArgumentException($"shadeIri {shadeIriString} does not start with userIri {userIriString}");
            var addition = shadeIriString.Substring(userIriString.Length);
            if (!shadeIriString.StartsWith('+') || shadeIriString.Contains('/'))
                throw new ArgumentException($"shaedIriAddition was not in the expected format: {addition}");
            return addition;
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

        public async Task<(IEnumerable<MediaModel> Creations, string Last)> GetPublicCreations(string? before = null)
        {
            // todo: appsettings
            var count = 50;
            var publicActivitiesTask = !string.IsNullOrEmpty(before)
                ? _publicCollectionGrain.GetReferencesAsync(ActivityType.Create, LongConverter.DecodeLong(before), count)
                : _publicCollectionGrain.GetReferencesAsync(ActivityType.Create, count);

            var random = new Random();
            var result = await publicActivitiesTask;
            var documentTasks = result.References.Select(async r =>
            {
                var document = await documentService.GetExpandedDocumentAsync(_instanceAuthor, r);
                if (!document.IsSuccessful)
                    return new Optional<MediaModel>();

                var objectObject = ActivityPubJsonNavigator.GetObject(document.Value);
                var objectId = ActivityPubJsonNavigator.GetId(objectObject);
                var objectDocument = await documentService.GetExpandedDocumentAsync(_instanceAuthor, Iri.FromUnencodedString(objectId));
                if (!objectDocument.IsSuccessful)
                    return new Optional<MediaModel>();

                var model = new MediaModel
                {
                    Depth = random.Next(0, 6)
                };

                var authorIri = Iri.FromUnencodedString(ActivityPubJsonNavigator.GetId(objectDocument.Value, JsonLdTypes.ATTRIBUTED_TO));
                var authorDocument = await documentService.GetExpandedDocumentAsync(_instanceAuthor, authorIri);

                if (authorDocument.IsSuccessful)
                {
                    var preferredUsername = ActivityPubJsonNavigator.TryGetValue(authorDocument.Value, JsonLdTypes.PREFERRED_USERNAME);
                    if (preferredUsername.HasValue)
                        model.AuthorFediverseHandle = GetFediverseUsername(preferredUsername.Value, authorIri.Host);

                    var name = ActivityPubJsonNavigator.TryGetValue(authorDocument.Value, JsonLdTypes.NAME);
                    if (name.HasValue)
                        model.AuthorName = name.Value;

                }
                model.AuthorFediverseHandle ??= authorIri.ToString();

                var timestamp = ActivityPubJsonNavigator.TryGetPublished(objectDocument.Value);
                if (timestamp.HasValue)
                    model.Timestamp = timestamp.Value.ToRelativeTime();

                var title = ActivityPubJsonNavigator.TryGetValue(objectDocument.Value, JsonLdTypes.NAME);
                if (title.HasValue)
                    model.Title = title.Value;

                var content = ActivityPubJsonNavigator.TryGetValue(objectDocument.Value, JsonLdTypes.CONTENT);
                if (content.HasValue)
                    model.Text = content.Value;

                return new(model);
            });

            var documents = (await Task.WhenAll(documentTasks))
                .Where(d => d.HasValue)
                .Select(d => d.Value);

            return (documents, LongConverter.EncodeLong(result.Last));
        }
    }
}
