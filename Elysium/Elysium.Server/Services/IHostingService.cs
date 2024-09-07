using DotNext;
using Elysium.Server.Models;
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
        LocalUri GetUriForLocalUser(string username);
        Result<string> GetLocalUserFromUri(LocalUri uri);
        public LocalUri GetLocalUserScopedUri(string username, string next);
    }
}
