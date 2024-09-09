using Elysium.Core.Models;
using Elysium.Persistence.Converters;
using Haondt.Core.Models;
using Haondt.Identity.StorageKey;
using Haondt.Persistence.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Elysium.Persistence.Services
{
    internal class DataObject
    {
        public Dictionary<string, object?> Values = [];
    }

    public class ElysiumFileStorage : IElysiumStorage
    {
        private string _dataFile;
        private readonly JsonSerializerSettings _serializerSettings;
        private DataObject? _dataCache;
        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

        // TODO: fix this Ioptions, use same style as sqlite settings
        public ElysiumFileStorage(IOptions<HaondtFileStorageSettings> options)
        {
            _dataFile = options.Value.DataFile;

            _serializerSettings = new JsonSerializerSettings();
            ConfigureSerializerSettings(_serializerSettings);
        }

        private static JsonSerializerSettings ConfigureSerializerSettings(JsonSerializerSettings settings)
        {
            settings.TypeNameHandling = TypeNameHandling.All;
            settings.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Error;
            settings.Formatting = Newtonsoft.Json.Formatting.Indented;
            settings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            settings.Converters.Add(new GenericStorageKeyJsonConverter());
            return settings;
        }

        private async Task TryAcquireSemaphoreAnd(Func<Task> func)
        {
            if (!await _semaphoreSlim.WaitAsync(1000))
                throw new Exception("Unable to acquire semaphore within the time limit");
            try { await func(); }
            finally { _semaphoreSlim.Release(); }
        }

        private async Task<T> TryAcquireSemaphoreAnd<T>(Func<Task<T>> func)
        {
            if (!await _semaphoreSlim.WaitAsync(1000))
                throw new Exception("Unable to acquire semaphore within the time limit");
            try { return await func(); }
            finally { _semaphoreSlim.Release(); }
        }

        private async Task<DataObject> GetDataAsync()
        {
            if (_dataCache != null)
                return _dataCache;

            if (!File.Exists(_dataFile))
            {
                _dataCache = new DataObject();
                return _dataCache;
            }

            using var reader = new StreamReader(_dataFile, new FileStreamOptions
            {
                Access = FileAccess.Read,
                BufferSize = 4096,
                Mode = FileMode.Open,
                Options = FileOptions.Asynchronous | FileOptions.SequentialScan
            });

            var text = await reader.ReadToEndAsync();

            _dataCache = JsonConvert.DeserializeObject<DataObject>(text, _serializerSettings) ?? new DataObject();
            return _dataCache;
        }

        private async Task SetDataAsync(DataObject data)
        {
            using var writer = new StreamWriter(_dataFile, new FileStreamOptions
            {
                Access = FileAccess.Write,
                BufferSize = 4096,
                Mode = FileMode.Create,
                Options = FileOptions.Asynchronous | FileOptions.SequentialScan
            });

            var text = JsonConvert.SerializeObject(data, _serializerSettings);
            await writer.WriteAsync(text);
            _dataCache = data;
        }

        public Task<bool> ContainsKey(StorageKey key) =>
            TryAcquireSemaphoreAnd(async () =>
            {
                var data = await GetDataAsync();
                return data.Values.ContainsKey(StorageKeyConvert.Serialize(key));
            });

        public Task<Result<StorageResultReason>> Delete(StorageKey key) =>
            TryAcquireSemaphoreAnd(async () =>
            {
                var data = await GetDataAsync();
                if (!data.Values.Remove(StorageKeyConvert.Serialize(key)))
                    return new(StorageResultReason.NotFound);
                await SetDataAsync(data);
                return new Result<StorageResultReason>();
            });

        public Task<Result<T, StorageResultReason>> Get<T>(StorageKey<T> key) =>
            TryAcquireSemaphoreAnd(async () =>
            {
                var data = await GetDataAsync();
                var stringKey = StorageKeyConvert.Serialize(key);
                if (!data.Values.TryGetValue(stringKey, out var value))
                    return new(StorageResultReason.NotFound);
                if (value is not T castedValue)
                    throw new InvalidCastException($"Cannot convert {key} to type {typeof(T)}");
                return new Result<T, StorageResultReason>(castedValue);
            });

        public Task Set<T>(StorageKey<T> key, T value) =>
            TryAcquireSemaphoreAnd(async () =>
            {
                var data = await GetDataAsync();
                data.Values[StorageKeyConvert.Serialize(key)] = value;
                await SetDataAsync(data);
            });


        public Task<Result<UserIdentity, StorageResultReason>> GetUserByNameAsync(string normalizedUsername) =>
            TryAcquireSemaphoreAnd(async () =>
            {
                var data = await GetDataAsync();
                foreach (var item in data.Values.Values)
                {
                    if (item is not UserIdentity userIdentity)
                        continue;
                    if (userIdentity.NormalizedUsername == normalizedUsername)
                        return new Result<UserIdentity, StorageResultReason>(userIdentity);
                }
                return new(StorageResultReason.NotFound);
            });
    }
}
