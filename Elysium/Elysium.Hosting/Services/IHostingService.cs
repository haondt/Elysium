using Elysium.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Hosting.Services
{
    public interface IHostingService
    {
        public string Host { get; }
    }
}
