using Elysium.GrainInterfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains.Services
{
    public class HttpRequestData
    {
        public required Uri Target { get; set; }
        public required IHttpMessageAuthor Author { get; set; }

    }
}
