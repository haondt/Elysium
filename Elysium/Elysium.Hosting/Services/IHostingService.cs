using DotNext;
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
        Result<LocalUri> GetUriForLocalUsername(string username);
        Result<LocalUri> GetUriForLocalizedUsername(string localizedUsername);
        //Task<Result<string>> GetUsernameFromLocalUriAsync(LocalUri uri);
        string GetUsernameFromLocalizedUsername(string username);
        Result<string> GetLocalizedUsernameFromUsername(string localizedUsername);
        public Result<LocalUri> GetLocalUserScopedUri(string username, string next);
        public LocalUri GetLocalUserScopedUri(LocalUri userUri, string next);
        public Task<Result<RemoteUri>> GetUriForRemoteUsernameAsync(string username);
        public Task<Result<Uri>> GetUriForUsernameAsync(string username);
        public string GetHost();
    }
}
