
using DotNext;

namespace Elysium.WebFinger.Services
{
    public interface IWebFingerService
    {
        Task<Result<JsonResourceDescriptor>> GetAsync(string resource);
    }
}
