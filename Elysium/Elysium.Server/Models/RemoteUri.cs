using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Server.Models
{
    public readonly record struct RemoteUri(Uri Uri)
    {
        public Uri Uri { get; init; } = Uri;
    }
}
