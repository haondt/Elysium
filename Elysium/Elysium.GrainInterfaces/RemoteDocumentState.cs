﻿using Elysium.Core.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces
{
    [GenerateSerializer]
    public class RemoteDocumentState
    {

        [Id(0)]
        public JToken? Value { get; set; }
        [Id(1)]
        public Iri? Owner { get; set; }
        [Id(2)]
        public DateTime? UpdatedOnUtc { get; set; }
    }
}
