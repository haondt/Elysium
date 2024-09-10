﻿using Elysium.Authentication.Constants;
using Elysium.Core.Models;
using Elysium.Persistence.Services;
using Elysium.Server.Services;
using Haondt.Identity.StorageKey;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Elysium.Hosting.Services
{
    public class HostingService : IHostingService
    {
        private readonly string _host;

        public HostingService(IOptions<HostingSettings> options)
        {
            _host = new IriBuilder { Host = options.Value.Host }.Iri.Host;
        }

        public bool IsLocalHost(Iri iri)
        {
            return iri.Host == _host;
        }

        // TODO: move to IActivityPubClientService
        //public Task<Result<string>> GetLocalizedUsernameFromLocalUriAsync(LocalUri iri)
        //{
        //    var path = iri.Iri.AbsolutePath;
        //    if (!path.StartsWith("/users/", StringComparison.OrdinalIgnoreCase))
        //        return new(new InvalidOperationException("invalid path"));
        //    var remaining = path.Substring("/users/".Length).Trim();
        //    if (remaining.Contains('/'))
        //        return new(new InvalidOperationException("invalid path"));

        //    var storageKey = _elysiumStorage.
        //    return new(remaining);
        //}

        public async Task<Iri> GetIriForUsernameAsync(string username)
        {
            if (username.Count(c => c == '@') != 1)
                throw new InvalidOperationException("unable to parse username");
            var host = username.Split('@')[^1];
            var partialUri = new IriBuilder { Host = host, Scheme = Uri.UriSchemeHttps }.Iri;
            if (IsLocalHost(partialUri))
                return GetIriForLocalUsername(username).Iri;
            return (await GetUriForRemoteUsernameAsync(username)).Iri;
        }

        // TODO: move to IActivityPubClientService
        //public Result<string> GetUsernameFromLocalUri(LocalUri iri)
        //{
        //    var localizedUsername = GetLocalizedUsernameFromLocalUri(iri);
        //    if (!localizedUsername.IsSuccessful)
        //        return localizedUsername;
        //    return new($"{localizedUsername.Value}@{_host}");
        //}

        public LocalIri GetIriForLocalUsername(string username)
        {
            _ = GetLocalizedUsernameFromUsername(username); // validation
            return GetIriForLocalizedUsername(username);
        }

        public LocalIri GetIriForLocalizedUsername(string localizedUsername)
        {
            return new LocalIri
            {
                Iri = new IriBuilder
                {
                    Host = _host,
                    Scheme = Uri.UriSchemeHttps,
                    Path = $"/users/{localizedUsername}"
                }.Iri
            };
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

            throw new NotImplementedException();

        }

        public LocalIri GetLocalUserScopedUri(string username, string next)
        {
            var userUri = GetIriForLocalUsername(username);
            return GetLocalUserScopedUri(userUri, next);
        }

        public LocalIri GetLocalUserScopedUri(LocalIri userUri, string next)
        {
            next = next.TrimStart('/');
            return new LocalIri { Iri = userUri.Iri.Concatenate(next) };
        }

        public string GetUsernameFromLocalizedUsername(string localizedUsername)
        {
            return $"{localizedUsername}@{_host}";
        }

        public string GetLocalizedUsernameFromUsername(string username)
        {
            var pattern = $"^([{AuthenticationConstants.ALLOWED_USERNAME_CHARACTERS.Replace("]", @"\]")}]+)@{Regex.Escape(_host)}$";
            var match = Regex.Match(username, pattern);
            if (!match.Success)
                throw new InvalidOperationException("unable to parse username as a local user");
            return match.Groups[1].Value;
        }

        public bool IsScopedToLocalUser(LocalIri iri, LocalIri user)
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

        public string Host => _host;
    }
}
