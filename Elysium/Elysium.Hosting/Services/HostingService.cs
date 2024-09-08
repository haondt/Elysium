using DotNext;
using Elysium.Authentication.Constants;
using Elysium.Core.Models;
using Elysium.Hosting.Models;
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
            _host = new UriBuilder { Host = options.Value.Host }.Uri.Host;
        }

        public bool IsLocalHost(Uri uri)
        {
            return uri.Host == _host;
        }

        // TODO: move to IActivityPubClientService
        //public Task<Result<string>> GetLocalizedUsernameFromLocalUriAsync(LocalUri uri)
        //{
        //    var path = uri.Uri.AbsolutePath;
        //    if (!path.StartsWith("/users/", StringComparison.OrdinalIgnoreCase))
        //        return new(new InvalidOperationException("invalid path"));
        //    var remaining = path.Substring("/users/".Length).Trim();
        //    if (remaining.Contains('/'))
        //        return new(new InvalidOperationException("invalid path"));

        //    var storageKey = _elysiumStorage.
        //    return new(remaining);
        //}

        public async Task<Result<Uri>> GetUriForUsernameAsync(string username)
        {
            if (username.Count(c => c == '@') != 1)
                return new(new InvalidOperationException("unable to parse username"));
            var host = username.Split('@')[^1];
            var partialUri = new UriBuilder { Host = host, Scheme = Uri.UriSchemeHttps }.Uri;
            if (IsLocalHost(partialUri))
            {
                var result = GetUriForLocalUsername(username);
                if (result.IsSuccessful)
                    return result.Value.Uri;
                return new(result.Error);
            }
            else
            {
                var result = await GetUriForRemoteUsernameAsync(username);
                if (result.IsSuccessful)
                    return result.Value.Uri;
                return new(result.Error);
            }
        }

        // TODO: move to IActivityPubClientService
        //public Result<string> GetUsernameFromLocalUri(LocalUri uri)
        //{
        //    var localizedUsername = GetLocalizedUsernameFromLocalUri(uri);
        //    if (!localizedUsername.IsSuccessful)
        //        return localizedUsername;
        //    return new($"{localizedUsername.Value}@{_host}");
        //}

        public Result<LocalUri> GetUriForLocalUsername(string username)
        {
            var localizedUsername = GetLocalizedUsernameFromUsername(username);
            if (!localizedUsername.IsSuccessful)
                return new(localizedUsername.Error);
            return new LocalUri
            {
                Uri = new UriBuilder
                {
                    Host = _host,
                    Scheme = Uri.UriSchemeHttps,
                    Path = $"/users/{localizedUsername.Value}"
                }.Uri
            };
        }
        public Task<Result<RemoteUri>> GetUriForRemoteUsernameAsync(string username)
        {
            //if (username.Count(c => c == '@') != 1)
            //    return new(new InvalidOperationException("unable to parse username as a remote user"));
            //var pattern = $"^([^@])@(.*)$";
            //var match = Regex.Match(username, pattern);
            //if (!match.Success)
            //    return new(new InvalidOperationException("unable to parse username as a remote user"));
            //try
            //{
            //    return new RemoteUri {  Uri = new Uri($"https://{}")}
            //}

            // TODO: this needs to use webfinger, maybe should be moved to IActivityPubClientService

            throw new NotImplementedException();

        }

        public Result<LocalUri> GetLocalUserScopedUri(string username, string next)
        {
            var userUri = GetUriForLocalUsername(username);
            if (!userUri.IsSuccessful)
                return userUri;

            next = next.TrimStart('/');
            return new LocalUri { Uri = new(userUri.Value.Uri, next) };
        }

        public string GetUsernameFromLocalizedUsername(string username)
        {
            return $"{username}@{_host}";
        }

        public Result<string> GetLocalizedUsernameFromUsername(string username)
        {
            var pattern = $"^([{AuthenticationConstants.ALLOWED_USERNAME_CHARACTERS.Replace("]", @"\]")}]+)@{Regex.Escape(_host)}$";
            var match = Regex.Match(username, pattern);
            if (!match.Success)
                return new(new InvalidOperationException("unable to parse username as a local user"));
            return new(match.Groups[1].Value);
        }

        public string GetHost()
        {
            return _host;
        }
    }
}
