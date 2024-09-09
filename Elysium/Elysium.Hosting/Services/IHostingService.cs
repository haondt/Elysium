using Elysium.Hosting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Server.Services
{
    public interface IHostingService
    {
        bool IsLocalHost(Uri uri);
        LocalUri GetUriForLocalUsername(string username);
        LocalUri GetUriForLocalizedUsername(string localizedUsername);
        //Task<Result<string>> GetUsernameFromLocalUriAsync(LocalUri uri);
        string GetUsernameFromLocalizedUsername(string username);
        string GetLocalizedUsernameFromUsername(string localizedUsername);
        public LocalUri GetLocalUserScopedUri(string username, string next);
        public LocalUri GetLocalUserScopedUri(LocalUri userUri, string next);
        public Task<RemoteUri> GetUriForRemoteUsernameAsync(string username);
        public Task<Uri> GetUriForUsernameAsync(string username);
        public string Host { get; }
    }
}
