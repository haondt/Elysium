using DotNext;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains.Services
{
    public class HostingService(IOptions<HostingSettings> options) : IHostingService
    {
        private readonly HostingSettings _hostingSettings = options.Value;   
        public bool IsLocalUserUri(Uri uri)
        {
            return uri.Host == _hostingSettings.Host;
        }
        public Result<string> GetLocalUserFromUri(Uri uri)
        {
            if (uri.Scheme != _hostingSettings.Scheme)
                return new(new InvalidOperationException("scheme mismatch"));
            if (uri.Host != _hostingSettings.Host)
                return new(new InvalidOperationException("host mismatch"));
            var path = uri.AbsolutePath;
            if (!path.StartsWith("/users/", StringComparison.OrdinalIgnoreCase))
                return new(new InvalidOperationException("invalid path"));
            var remaining = path.Substring("/users/".Length).Trim();
            if (remaining.Contains('/'))
                return new(new InvalidOperationException("invalid path"));
            return new(remaining.ToLower().Trim());
        }
        public Uri GetUriForLocalUser(string username)
        {
            return new UriBuilder
            {
                Host = _hostingSettings.Host,
                Scheme = _hostingSettings.Scheme,
                Path = $"/users/{username}"
            }.Uri;
        }

        public Uri GetLocalUserScopedUri(string username, string next)
        {
            var userUri = GetUriForLocalUser(username);
            next = next.TrimStart('/');
            return new Uri(userUri, next);
        }
    }
}
