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
        Result<LocalUri> GetUriForLocalUser(string username);
        Result<string> GetUsernameFromLocalUri(LocalUri uri);
        public Result<LocalUri> GetLocalUserScopedUri(string username, string next);
    }
}
