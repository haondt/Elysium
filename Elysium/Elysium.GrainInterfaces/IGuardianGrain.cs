using Elysium.Hosting.Models;
using Haondt.Core.Models;
using Newtonsoft.Json.Linq;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces
{
    public interface IGuardianGrain : IGrainWithGuidKey
    {
        Task<Result<GuardianReason>> TryCreateDocumentAsync(
            LocalUri actor, 
            LocalUri objectUri, 
            JObject compactedObject,
            List<Uri> bto, // note: bto / bcc might be empty lists, in which case they should not be persisted
            List<Uri> bcc);
    }
}
