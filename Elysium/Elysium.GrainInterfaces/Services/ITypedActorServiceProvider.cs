using DotNext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces.Services
{
    public interface ITypedActorServiceProvider
    {
        Result<ITypedActorService> GetService(Uri id);
    }
}
