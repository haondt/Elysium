using Elysium.Core.Models;
using Orleans.Concurrency;

namespace Elysium.GrainInterfaces.Services
{
    public interface IHttpMessageAuthor
    {
        Task<string> GetKeyIdAsync();
        Task<LocalIri> GetIriAsync();
        [AlwaysInterleave]
        Task<string> SignAsync(string stringToSign);
    }
}