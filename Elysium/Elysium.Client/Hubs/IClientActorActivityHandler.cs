using Elysium.GrainInterfaces.Client;
using Haondt.Core.Models;

namespace Elysium.Client.Hubs
{
    public interface IClientActorActivityHandler
    {
        string ClientType { get; }
        Task<Optional<(string Method, Optional<object> Arg)>> HandleAsync(ClientIncomingActivityDetails details);
    }
}
