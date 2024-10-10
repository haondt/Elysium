namespace Elysium.Client.Hubs
{
    public class ClientActorActivityHandlerFactory(IEnumerable<IClientActorActivityHandler> handlers) : IClientActorActivityHandlerFactory
    {
        public IClientActorActivityHandler Create(string clientType)
        {
            return handlers.First(h => h.ClientType.Equals(clientType));
        }
    }
}
