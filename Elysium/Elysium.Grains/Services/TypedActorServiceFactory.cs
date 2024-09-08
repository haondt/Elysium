using DotNext;
using Elysium.GrainInterfaces.Services;
using Elysium.Server.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains.Services
{
    public class TypedActorServiceFactory(IServiceProvider serviceProvider, IHostingService hostingService) : ITypedActorServiceProvider
    {
        public Result<ITypedActorService> GetService(Uri id)
        {
            // todo: some magic logic that decides this uri == a useridentity, and returns 
                //return new(serviceProvider.GetRequiredService<IUserActorService>());
            throw new NotImplementedException();
        }
    }
}
