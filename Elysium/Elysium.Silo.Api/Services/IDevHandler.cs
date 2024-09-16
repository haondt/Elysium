
namespace Elysium.Silo.Api.Services
{
    public interface IDevHandler
    {
        Task CreateForLocal(DevLocalActivityPayload payload);
    }
}