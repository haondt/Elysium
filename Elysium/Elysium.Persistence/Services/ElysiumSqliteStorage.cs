using Elysium.Core.Models;
using Elysium.Core.Services;
using Elysium.Persistence.Converters;
using Haondt.Core.Models;
using Haondt.Identity.StorageKey;
using Haondt.Persistence.Services;
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
        private readonly IElysiumStorageKeyConverter _storageKeyConverter;

        public ElysiumSqliteStorage(IOptions<ElysiumPersistenceSettings> options, IElysiumStorageKeyConverter storageKeyConverter)
        {
            _settings = options.Value.SqliteStorageSettings;
            _serializerSettings = new JsonSerializerSettings();
            _connectionString = new SqliteConnectionStringBuilder()
            {
                DataSource = _settings.DatabasePath,
                Mode = SqliteOpenMode.ReadWriteCreate,
                Cache = SqliteCacheMode.Private
            }.ToString();
            _storageKeyConverter = storageKeyConverter;
            ConfigureSerializerSettings(_serializerSettings);
            GetExistingTables();
        }

        private JsonSerializerSettings ConfigureSerializerSettings(JsonSerializerSettings settings)
        {
            settings.TypeNameHandling = TypeNameHandling.Auto;
            settings.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Error;
            settings.Formatting = Newtonsoft.Json.Formatting.None;
            settings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            settings.Converters.Add(new GenericStorageKeyJsonConverter(_storageKeyConverter));
            return settings;
        }

        private async Task EnableWalModeAsync(SqliteConnection connection)
        {
            using var command = connection.CreateCommand();
            command.CommandText = "PRAGMA journal_mode=WAL;";
            await command.ExecuteNonQueryAsync();
        }

        private async Task GetExistingTables()
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            await EnableWalModeAsync(connection);
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
            //var tableName = tableType.AssemblyQualifiedName ?? throw new InvalidOperationException("Type name is null");
            var tableName = SimpleTypeConverter.TypeToString(tableType);
            tableName = SanitizeTableName(tableName);

            if (_existingTables.ContainsKey(tableName)) return tableName;

            var createTableQuery = tableType == typeof(UserIdentity)
                ? $@"
                    CREATE TABLE IF NOT EXISTS {tableName} (
                        Key TEXT PRIMARY KEY,
                        KeyString TEXT NOT NULL,
                        Value TEXT NOT NULL,
                        NormalizedUsername TEXT
                    )"
                : $@"
                    CREATE TABLE IF NOT EXISTS {tableName} (
                        Key TEXT PRIMARY KEY,
                        KeyString TEXT NOT NULL,
                        Value TEXT NOT NULL
                    )";

            using var command = connection.CreateCommand();
            command.CommandText = createTableQuery;
            await command.ExecuteNonQueryAsync();

            _existingTables[tableName] = true;

            return tableName;
        }

        public async Task<Result<T, StorageResultReason>> Get<T>(StorageKey<T> key)
        {
            var keyString = _storageKeyConverter.Serialize(key);
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            await EnableWalModeAsync(connection);
            var table = await GetOrCreateTableAsync(key.Type, connection);
            var query = $"SELECT Value FROM {table} WHERE Key = @key";
            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@key", keyString);
            var result = await command.ExecuteScalarAsync();

            if (result == null)
                return new(StorageResultReason.NotFound);
            var resultString = result.ToString() 
                ?? throw new JsonException("unable to deserialize result");
            var value = JsonConvert.DeserializeObject<T>(resultString, _serializerSettings) 
                ?? throw new JsonException("unable to deserialize result");
            return new(value);
        }

        public async Task<bool> ContainsKey(StorageKey key)
        {
            var keyString = _storageKeyConverter.Serialize(key);
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            await EnableWalModeAsync(connection);
            var table = await GetOrCreateTableAsync(key.Type, connection);
            string query = $"SELECT COUNT(1) FROM {table} WHERE Key = @key";
            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@key", keyString);
            var count = await command.ExecuteScalarAsync();
            if (count is not long longCount)
                throw new JsonException("unable to deserialize result");
            return longCount > 0;
        }

        public Task Set<T>(StorageKey<T> key, T value)
        {
            return SetMany([(key, value!)]);
        }

        public async Task<Result<StorageResultReason>> Delete(StorageKey key)
        {
            var keyString = _storageKeyConverter.Serialize(key);
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            await EnableWalModeAsync(connection);
            var table = await GetOrCreateTableAsync(key.Type, connection);
            string query = $"DELETE FROM {table} WHERE Key = @key";
            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@key", keyString);
            await command.ExecuteNonQueryAsync();
            return new();
        }

        public async Task<Result<UserIdentity, StorageResultReason>> GetUserByNameAsync(string normalizedUsername)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            await EnableWalModeAsync(connection);
            var table = await GetOrCreateTableAsync(typeof(UserIdentity), connection);
            var query = $" SELECT Value FROM {table} WHERE NormalizedUsername = @normalizedUsername";
            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@normalizedUsername", normalizedUsername);
            using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
                return new(StorageResultReason.NotFound);

            var valueString = reader.GetString(0);
            var resultUserIdentity = JsonConvert.DeserializeObject<UserIdentity>(valueString, _serializerSettings)
                ?? throw new JsonException("Unable to deserialize result");
            return new(resultUserIdentity);
        }

        private string SanitizeTableName(string tableName)
        {
            // Escape any double quotes by replacing them with two double quotes
            var sanitized = tableName.Replace("\"", "\"\"");
    
            // Surround the sanitized table name with double quotes
            return $"\"{sanitized}\"";
        }

        public async Task SetMany(List<(StorageKey Key, object Value)> values)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            await EnableWalModeAsync(connection);
            using var transaction = await connection.BeginTransactionAsync();

            var commands = new Dictionary<Type, (SqliteCommand Command, Action<StorageKey, object> SetParameters)>(); ;
            async Task<(SqliteCommand Command, Action<StorageKey,  object> SetParameters)> PreparedCommandAsync(Type keyType)
            {
                var table = await GetOrCreateTableAsync(keyType, connection);

                var setParametersList = new List<Action<StorageKey,  object>>();

                var command = connection.CreateCommand();
                if (keyType == typeof(UserIdentity))
                {
                    if (_settings.StoreKeyStrings)
                        command.CommandText = $"INSERT OR REPLACE INTO {table} (Key, KeyString, Value, NormalizedUsername) " +
                            $"VALUES (@key, @keyString, @value, @normalizedUsername)";
                    else
                        command.CommandText = $"INSERT OR REPLACE INTO {table} (Key, Value, NormalizedUsername) " +
                            $"VALUES (@key, @value, @normalizedUsername)";
                    var normalizedUsernameParameter = command.CreateParameter();
                    normalizedUsernameParameter.ParameterName = "@normalizedUsername";
                    command.Parameters.Add(normalizedUsernameParameter);
                    setParametersList.Add((key, value) =>
                    {
                        if (value is not UserIdentity userIdentity)
                            throw new InvalidOperationException($"storage key {key} has type UserIdentity but the value was not a UserIdentity");
                        normalizedUsernameParameter.Value = userIdentity.NormalizedUsername;
                    });
                }
                else if (_settings.StoreKeyStrings)
                   command.CommandText = $"INSERT OR REPLACE INTO {table} (Key, KeyString, Value) VALUES (@key, @keyString, @value)";
                else
                   command.CommandText = $"INSERT OR REPLACE INTO {table} (Key, Value) VALUES (@key, @value)";

                var keyParameter = command.CreateParameter();
                keyParameter.ParameterName = "@key";
                command.Parameters.Add(keyParameter);
                setParametersList.Add((key, _) =>
                {
                    keyParameter.Value = _storageKeyConverter.Serialize(key);
                });
                if (_settings.StoreKeyStrings)
                {
                    var keyStringParameter = command.CreateParameter();
                    keyStringParameter.ParameterName = "@keyString";
                    command.Parameters.Add(keyStringParameter);
                    setParametersList.Add((key, _) =>
                    {
                        keyStringParameter.Value = key.ToString();
                    });
                }
                var valueParameter = command.CreateParameter();
                valueParameter.ParameterName = "@value";
                command.Parameters.Add(valueParameter);
                setParametersList.Add((_, value) =>
                {
                    valueParameter.Value = JsonConvert.SerializeObject(value, _serializerSettings);
                });

                return (command, (key, value) =>
                {
                    foreach (var action in setParametersList)
                        action(key, value);
                });
            }

            foreach(var (key, value) in values)
            {
                var keyString = _storageKeyConverter.Serialize(key);
                var valueString = JsonConvert.SerializeObject(value, _serializerSettings);

                if (!commands.ContainsKey(key.Type))
                    commands[key.Type] = await PreparedCommandAsync(key.Type);

                var (command, setParameters) = commands[key.Type];
                setParameters(key, value);
                await command.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();
        }

        public async Task<List<Result<(StorageKey Key, object Value), StorageResultReason>>> GetMany(List<StorageKey> keys)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            await EnableWalModeAsync(connection);


            var resultTasks = keys.Select(async key =>
            {
                var keyString = _storageKeyConverter.Serialize(key);
                var table = await GetOrCreateTableAsync(key.Type, connection);
                var query = $"SELECT Value FROM {table} WHERE Key = @key";
                using var command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@key", keyString);
                var result = await command.ExecuteScalarAsync();

                if (result == null)
                    return new(StorageResultReason.NotFound);
                var resultString = result.ToString()
                    ?? throw new JsonException("unable to deserialize result");
                var value = JsonConvert.DeserializeObject(resultString, key.Type, _serializerSettings)
                    ?? throw new JsonException("unable to deserialize result");
                return new Result<(StorageKey, object), StorageResultReason>((key, value));
            });

            return (await Task.WhenAll(resultTasks)).ToList();
        }
    }
}

