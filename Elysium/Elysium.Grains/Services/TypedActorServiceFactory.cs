using DotNext;
using Elysium.GrainInterfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains.Services
{
    public class TypedActorServiceFactory(IServiceProvider serviceProvider) : ITypedActorServiceFactory
    {
        public Result<ITypedActorService> Create(ActorType actorType)
        {
            if (actorType == ActorType.Unknown)
                return new(new InvalidOperationException("actor type is unknown"));
            if (actorType == ActorType.Person)
                return new(serviceProvider.GetRequiredService<IPersonActorService>());
            return new(new InvalidOperationException($"actor type {actorType} is unknown"));
        }
    }
}
