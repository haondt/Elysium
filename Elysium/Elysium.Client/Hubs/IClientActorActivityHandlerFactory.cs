namespace Elysium.Client.Hubs
{
    public interface IClientActorActivityHandlerFactory
    {
        IClientActorActivityHandler Create(string clientType);
    }
}
