﻿using Elysium.GrainInterfaces;
using Haondt.Core.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains
{
    public class ExpandedDocumentCacheGrain : CacheGrain<DocumentState<JArray>>, IExpandedDocumentCacheGrain
    {
    }
}
