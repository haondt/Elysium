using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Server.Services
{
    public class HostingSettings
    {
        public required string Host { get; set; }
        /// <summary>
        /// http/https
        /// </summary>
        public required string Scheme { get; set; }
    }
}
