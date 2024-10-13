using Elysium.GrainInterfaces.Queueing;
using Elysium.Silo.Api.Services;

namespace Elysium.Silo.Api.SiloStartupParticipants
{
    public class QueueStartupParticipant(IGrainFactory grainFactory) : ISiloStartupParticipant
    {
        public Task OnStartupAsync()
        {
            var startupGrain = grainFactory.GetGrain<IQueueStartupGrain>(Guid.NewGuid());
            return startupGrain.EnsureActivatedAsync();
        }
    }
}
