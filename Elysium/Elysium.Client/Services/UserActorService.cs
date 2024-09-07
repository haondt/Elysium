using DotNext;
using Elysium.GrainInterfaces.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Client.Services
{
    public class UserActorService : IPersonActorService
    {
        public ActorType ActorType => ActorType.Person;


        public Task<byte[]> GetPublicKeyAsync()
        {
            //if (!_userIdentity.IsSuccessful)
            //    return new(_userIdentity.Error);
            //var username = _userIdentity.Value.Username ?? _id;
            throw new NotImplementedException();
        }

        public Task<byte[]> GetSigningKeyAsync()
        {
        //var storageKey = UserIdentity.GetStorageKey(_id);
        //_userIdentity = await _storage.Get(storageKey);
        //if (!_userIdentity.IsSuccessful)
        //    _signingKey = new(_userIdentity.Error);
        //else
        //{
        //    var encryptedPrivateKey = _userIdentity.Value.EncryptedPrivateKey;
        //    _signingKey = _cryptoService.DecryptPrivateKey(encryptedPrivateKey);
        //}
            throw new NotImplementedException();
        }

        public Task<Result<JObject>> GenerateDocumentAsync()

        {
            if (!_typedActorService.IsSuccessful)
                return new(_typedActorService.Error);
            return new(await _typedActorService.Value.GetSigningKeyAsync());

            // need to add the public key
            // for multibase need the right contexts
            // 
            // "https://www.w3.org/ns/activitystreams",
            //  "https://www.w3.org/ns/did/v1",
            //   "https://w3id.org/security/multikey/v1"
            // see https://web.archive.org/web/20221218063101/https://web-payments.org/vocabs/security#publicKey
            // and https://swicg.github.io/activitypub-http-signature/#how-to-obtain-a-signature-s-public-key
            // and https://www.w3.org/TR/controller-document/#dfn-publickeymultibase

            var builder = new ActivityPubJsonBuilder()
                .Id(_hostingService.GetUriForLocalUser(username))
                .Inbox(_hostingService.GetLocalUserScopedUri(username, "inbox"))
                .Outbox(_hostingService.GetLocalUserScopedUri(username, "outbox"))
                .Followers(_hostingService.GetLocalUserScopedUri(username, "followers"))
                .Following(_hostingService.GetLocalUserScopedUri(username, "following"));

            //if (!string.IsNullOrEmpty(_userIdentity.Value.Username))
            //    builder = builder.PreferredUsername(_userIdentity.Value.Username);

            var expandedActor = builder.Build();
            if (!expandedActor.IsSuccessful)
                return new(expandedActor.Error);

            return await _jsonLdService.CompactAsync(expandedActor.Value);


        }

    }
}
