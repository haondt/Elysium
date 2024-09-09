using Elysium.GrainInterfaces.Services;
using Elysium.Hosting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces
{
    public interface ILocalActorWorkerGrain : IGrain<LocalIri>
    {
    }
}
