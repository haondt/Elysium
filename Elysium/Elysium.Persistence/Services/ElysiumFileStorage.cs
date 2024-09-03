using DotNext;
using Elysium.Core.Models;
using Haondt.Identity.StorageKey;
using Haondt.Persistence.Services;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Elysium.Persistence.Services
{
    internal class DataObject
    {
        public Dictionary<string, object?> Values = [];
    }

    public class ElysiumFileStorage : FileStorage, IElysiumStorage
    {
        public Task<Result<UserIdentity>> GetUserByNameAsync(string normalizedUsername) =>
            TryAcquireSemaphoreAnd(async () =>
            {
                var data = await GetDataAsync();
                foreach (var item in data.Values.Values)
                {
                    if (item is not UserIdentity userIdentity)
                        continue;
                    if (userIdentity.NormalizedUsername == normalizedUsername)
                        return new Result<UserIdentity>(userIdentity);
                }
                return new Result<UserIdentity>(new KeyNotFoundException(normalizedUsername));
            });
    }
}
