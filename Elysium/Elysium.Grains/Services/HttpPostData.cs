using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains.Services
{
    public class HttpPostData : HttpRequestData
    {
        public required string JsonLdPayload { get; set; }
    }
}
