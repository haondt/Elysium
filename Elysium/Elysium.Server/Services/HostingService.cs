using DotNext;
using Elysium.Server.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Server.Services
{
    public class HostingService(IOptions<HostingSettings> options) : IHostingService
    {
        private readonly HostingSettings _hostingSettings = options.Value;
        public bool IsLocalHost(Uri uri)
        {
            return uri.Host == _hostingSettings.Host;
        }

        public Result<string> GetLocalUserFromUri(LocalUri uri)
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
            return new(remaining.ToLower().Trim());
        }
        public LocalUri GetUriForLocalUser(string username)
        {
            return new LocalUri
            {
                Uri = new UriBuilder
                {
                    Host = _hostingSettings.Host,
                    Scheme = _hostingSettings.Scheme,
                    Path = $"/users/{username}"
                }.Uri
            };
        }

        public LocalUri GetLocalUserScopedUri(string username, string next)
        {
            var userUri = GetUriForLocalUser(username);
            next = next.TrimStart('/');
            return new LocalUri { Uri = new(userUri.Uri, next) };
        }
    }
}
