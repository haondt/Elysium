using DotNext;
using Elysium.Authentication.Constants;
using Elysium.Hosting.Models;
using Elysium.Server.Services;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Elysium.Hosting.Services
{
    public class HostingService(IOptions<HostingSettings> options) : IHostingService
    {
        private readonly HostingSettings _hostingSettings = options.Value;
        public bool IsLocalHost(Uri uri)
        {
            return uri.Host == _hostingSettings.Host;
        }

        public Result<string> GetLocalizedUsernameFromLocalUri(LocalUri uri)
        {
            if (uri.Uri.Scheme != _hostingSettings.Scheme)
                return new(new InvalidOperationException("scheme mismatch"));
            if (uri.Uri.Host != _hostingSettings.Host)
                return new(new InvalidOperationException("host mismatch"));
            var path = uri.Uri.AbsolutePath;
            if (!path.StartsWith("/users/", StringComparison.OrdinalIgnoreCase))
                return new(new InvalidOperationException("invalid path"));
            var remaining = path.Substring("/users/".Length).Trim();
            if (remaining.Contains('/'))
                return new(new InvalidOperationException("invalid path"));
            return new(remaining);
        }

        public Result<string> GetUsernameFromLocalUri(LocalUri uri)
        {
            var localizedUsername = GetLocalizedUsernameFromLocalUri(uri);
            if (!localizedUsername.IsSuccessful)
                return localizedUsername;
            return new($"{localizedUsername.Value}@{_hostingSettings.Host}");
        }

        public Result<LocalUri> GetUriForLocalUser(string username)
        {
            // COMEHERE
            var pattern = $"^([{AuthenticationConstants.ALLOWED_USERNAME_CHARACTERS.Replace("]", @"\]")}])@{Regex.Escape(_hostingSettings.Host)}$";
            var match = Regex.Match(username, pattern);
            if (!match.Success)
                return new(new InvalidOperationException("unable to parse username as a local user"));

            return new LocalUri
            {
                Uri = new UriBuilder
                {
                    Host = _hostingSettings.Host,
                    Scheme = _hostingSettings.Scheme,
                    Path = $"/users/{match.Groups[1].Value}"
                }.Uri
            };
        }

        public Result<LocalUri> GetLocalUserScopedUri(string username, string next)
        {
            var userUri = GetUriForLocalUser(username);
            if (!userUri.IsSuccessful)
                return userUri;

            next = next.TrimStart('/');
            return new LocalUri { Uri = new(userUri.Value.Uri, next) };
        }
    }
}
