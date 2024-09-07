using DotNext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains.Services
{
    public interface IHostingService
    {
        Uri GetUriForLocalUser(string username);
        Result<string> GetLocalUserFromUri(Uri uri);
        public Uri GetLocalUserScopedUri(string username, string next);
    }
}
