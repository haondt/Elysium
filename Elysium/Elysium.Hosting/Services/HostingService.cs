﻿using Elysium.Core.Models;
using Elysium.Server.Services;
using Microsoft.Extensions.Options;

namespace Elysium.Hosting.Services
{
    public class HostingService : IHostingService
    {
        private readonly string _host;

        public HostingService(IOptions<HostingSettings> options)
        {
            _host = new IriBuilder { Host = options.Value.Host }.Iri.Host;
        }

        public string Host => _host;
    }
}
