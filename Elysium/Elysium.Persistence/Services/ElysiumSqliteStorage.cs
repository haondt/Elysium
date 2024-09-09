using DotNext;
using Elysium.Core.Models;
using Elysium.Persistence.Converters;
using Haondt.Identity.StorageKey;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Persistence.Services
{
    public class ElysiumSqliteStorage : IElysiumStorage
    {
        private readonly ElysiumSqliteStorageSettings _settings;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly string _connectionString;
        private readonly ConcurrentDictionary<string, bool> _existingTables = [];

        public ElysiumSqliteStorage(IOptions<ElysiumPersistenceSettings> options)
        {
            _settings = options.Value.SqliteStorageSettings;
            _serializerSettings = new JsonSerializerSettings();
            _connectionString = new SqliteConnectionStringBuilder()
            {
                DataSource = _settings.DatabasePath,
                Mode = SqliteOpenMode.ReadWriteCreate,
                Cache = SqliteCacheMode.Private
            }.ToString();
            ConfigureSerializerSettings(_serializerSettings);
            GetExistingTables();
        }

        private static JsonSerializerSettings ConfigureSerializerSettings(JsonSerializerSettings settings)
        {
            settings.TypeNameHandling = TypeNameHandling.All;
            settings.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Error;
            settings.Formatting = Newtonsoft.Json.Formatting.None;
            settings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            settings.Converters.Add(new GenericStorageKeyJsonConverter());
            return settings;
        }

        private void GetExistingTables()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var query = "SELECT name FROM sqlite_master where type='table'";
            using var command = new SqliteCommand(query, connection);
            using var reader = command.ExecuteReader();
            while(reader.Read())
            {
                var tableName = reader.GetString(0);
                if (!string.IsNullOrEmpty(tableName))
                    _existingTables[tableName] = true;
            }
        }

        private async Task<string> GetOrCreateTableAsync(Type tableType, SqliteConnection connection)
        {
            var tableName = tableType.AssemblyQualifiedName ?? throw new InvalidOperationException("Type name is null");

            if (_existingTables.ContainsKey(tableName)) return tableName;

            var createTableQuery = tableType == typeof(UserIdentity)
                ? $@"
                    CREATE TABLE IF NOT EXISTS [{tableName}] (
                        Key TEXT PRIMARY KEY,
                        Value TEXT NOT NULL,
                        NormalizedUsername TEXT
                    )"
                : $@"
                    CREATE TABLE IF NOT EXISTS [{tableName}] (
                        Key TEXT PRIMARY KEY,
                        Value TEXT NOT NULL
                    )";

            using var command = new SqliteCommand(createTableQuery, connection);
            await command.ExecuteNonQueryAsync();

            _existingTables[tableName] = true;

            return tableName;
        }

        public async Task<Result<T>> Get<T>(StorageKey<T> key)
        {
            try
            {
                var keyString = StorageKeyConvert.Serialize(key);
                using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();
                var table = await GetOrCreateTableAsync(key.Type, connection);
                var query = $"SELECT Value FROM [{table}] WHERE Key = @key";
                using var command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@key", keyString);
                var result = await command.ExecuteScalarAsync();

                if (result == null)
                    return new(new KeyNotFoundException());
                var resultString = result.ToString();
                if (resultString == null)
                    return new(new JsonException("unable to deserialize result"));
                var value = JsonConvert.DeserializeObject<T>(resultString, _serializerSettings);
                if (value == null)
                    return new(new JsonException("unable to deserialize result"));
                return value;
            }
            catch (Exception ex)
            {
                return new(ex);
            }
        }

        public async Task<Result<bool>> ContainsKey(StorageKey key)
        {
            try
            {
                var keyString = StorageKeyConvert.Serialize(key);
                using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();
                var table = await GetOrCreateTableAsync(key.Type, connection);
                string query = $"SELECT COUNT(1) FROM [{table}] WHERE Key = @key";
                using var command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@key", keyString);
                var count = await command.ExecuteScalarAsync();
                if (count is not long longCount)
                    return new(new JsonException("unable to deserialize result"));
                return longCount > 0;
            }
            catch (Exception ex)
            {
                return new(ex);
            }
        }

        public async Task<Optional<Exception>> Set<T>(StorageKey<T> key, T value)
        {
            try
            {
                var keyString = StorageKeyConvert.Serialize(key);
                var valueString = JsonConvert.SerializeObject(value, _serializerSettings);

                using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();
                var table = await GetOrCreateTableAsync(key.Type, connection);

                string query;
                Action<SqliteCommand>? setAdditionalParameter = null; 
                if (typeof(T) == typeof(UserIdentity))
                {
                    if (value is not UserIdentity userIdentity)
                        return new(new InvalidOperationException("storage key type was UserIdentity but the value was not a UserIdentity"));

                    query = $"INSERT OR REPLACE INTO [{table}] (Key, Value, NormalizedUsername) " +
                        $"VALUES (@key, @value, @normalizedUsername)";
                    setAdditionalParameter = c => c.Parameters.AddWithValue("@normalizedUsername", userIdentity.NormalizedUsername ?? (object)DBNull.Value);
                }
                else
                   query = $"INSERT OR REPLACE INTO [{table}] (Key, Value) VALUES (@key, @value)";

                using var command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@key", keyString);
                command.Parameters.AddWithValue("@value", valueString);
                setAdditionalParameter?.Invoke(command);
                await command.ExecuteNonQueryAsync();

                return Optional<Exception>.None;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        public async Task<Optional<Exception>> Delete(StorageKey key)
        {
            try
            {
                var keyString = StorageKeyConvert.Serialize(key);
                using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();
                var table = await GetOrCreateTableAsync(key.Type, connection);
                string query = $"DELETE FROM [{table}] WHERE Key = @key";
                using var command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@key", keyString);
                await command.ExecuteNonQueryAsync();

                return Optional<Exception>.None;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        public async Task<Result<UserIdentity>> GetUserByNameAsync(string normalizedUsername)
        {
            try
            {
                using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();
                var table = await GetOrCreateTableAsync(typeof(UserIdentity), connection);
                var query = $" SELECT Value FROM [{table}] WHERE NormalizedUsername = @normalizedUsername";
                using var command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@normalizedUsername", normalizedUsername);
                using var reader = await command.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                    return new(new KeyNotFoundException($"No user found with normalized username '{normalizedUsername}'"));

                var valueString = reader.GetString(0);
                var resultUserIdentity = JsonConvert.DeserializeObject<UserIdentity>(valueString, _serializerSettings);
                if (resultUserIdentity == null)
                    return new(new JsonException("Unable to deserialize result"));
                return resultUserIdentity;
            }
            catch (Exception ex)
            {
                return new(ex);
            }
        }
    }
}

