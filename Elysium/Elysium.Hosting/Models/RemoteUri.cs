﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Hosting.Models
{
    [GenerateSerializer, Immutable]
    public readonly record struct RemoteUri(Uri Uri)
    {
        public Uri Uri { get; init; } = Uri;
    }
}
