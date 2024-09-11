using Elysium.Core.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains.Services
{
    public interface IActorDocumentGenerator
    {
        Task<JObject> GenerateAsync(LocalIri target);
    }
}
