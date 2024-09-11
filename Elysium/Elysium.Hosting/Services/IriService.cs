using Elysium.Core.Models;
using Haondt.Core.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elysium.Core.Extensions;

namespace Elysium.Hosting.Services
{
    public class IriService(IHostingService hostingService) : IIriService
    {
        public LocalIri InstanceActorIri { get; } = new()
        {
            Iri = new IriBuilder
            {
                Host = hostingService.Host,
                Path = hostingService.Host,
            }.Iri
        };

        public LocalIri GetIriForLocalizedActorname(string localizedUsername)
        {
            return new LocalIri
            {
                Iri = new IriBuilder
                {
                    Host = hostingService.Host,
                    Scheme = Uri.UriSchemeHttps,
                    Path = $"/users/{localizedUsername}"
                }.Iri
            };
        }

        public string GetActornameFromLocalizedActorname(string localizedUsername)
        {
            return $"{localizedUsername}@{hostingService.Host}";
        }

        /// <summary>
        /// Check if the <paramref name="iri"/> is scoped to the <paramref name="user"/>.<br/> 
        /// For example, "https://localhost.com/users/terry/status/95538 is scoped
        /// to "https://localhost.com/users/terry"
        /// </summary>
        /// <param name="iri"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool IsScopedToLocalActor(LocalIri iri, LocalIri user)
        {
            var uriString = iri.Iri.ToString();
            var userString = user.Iri.ToString();

            if (uriString.Length < userString.Length)
                return false;
            if (uriString == userString)
                return true;
            if (!uriString.StartsWith(userString))
                return false;
            var uriPath = uriString.Substring(userString.Length);
            return uriPath.StartsWith('/');
        }
        public bool IsScopedToLocalActor(LocalIri iri)
        {
            var uriString = iri.Iri.ToString();
            var scopeString = GetIriForLocalizedActorname("").ToString();
            if (uriString == scopeString)
                return false;
            if (uriString.StartsWith($"{scopeString}/"))
                return true;
            return false;
        }

        public LocalIri GetActorScopedObjectIri(LocalIri user, string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("cannot be empty", nameof(id));
            return new LocalIri { Iri = user.Iri.Concatenate($"objects/{id}") };
        }

        public LocalIri GetAnonymousObjectIri(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("cannot be empty", nameof(id));
            return new LocalIri
            {
                Iri = new IriBuilder
                {
                    Host = hostingService.Host,
                    Scheme = Uri.UriSchemeHttps,
                    Path = $"/objects/{id}"
                }.Iri
            };
        }

        public LocalIri GetActorScopedActivityIri(LocalIri user, string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("cannot be empty", nameof(id));
            return new LocalIri { Iri = user.Iri.Concatenate($"activities/{id}") };
        }

        public LocalActorIriCollection GetLocalActorIris(LocalIri iri)
        {
            return new LocalActorIriCollection
            {
                Inbox = iri.Concatenate("inbox"),
                Outbox = iri.Concatenate("outbox"),
                Followers = iri.Concatenate("followers"),
                Following = iri.Concatenate("following"),
                PublicKey = new LocalIri { Iri = Iri.FromUnencodedString($"{iri}#main-key") }
            };
        }
    }
}
