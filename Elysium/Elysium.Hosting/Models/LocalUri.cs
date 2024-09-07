﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Hosting.Models
{
    public readonly record struct LocalUri(Uri Uri)
    {
        public Uri Uri { get; init; } = Uri;
    }
}
