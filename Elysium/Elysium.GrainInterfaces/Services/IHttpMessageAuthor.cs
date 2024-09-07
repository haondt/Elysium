using DotNext;

namespace Elysium.GrainInterfaces.Services
{
    public interface IHttpMessageAuthor
    {
        Task<string> GetKeyIdAsync();
        Task<Result<string>> SignAsync(string stringToSign);
    }
}