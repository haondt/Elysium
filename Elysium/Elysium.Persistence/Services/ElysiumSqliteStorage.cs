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
    public class TableDescriptor
    {
        private HashSet<string> _columns = [];
        private List<string> _orderedColumns = [];
        public HashSet<string> Columns { get { return _columns; } }
        public List<string> OrderedColumns
        {
            get {  return _orderedColumns; }
            set
            {
                _orderedColumns = value;
                _columns = new (value);
            }
        }
    }

    public class ElysiumSqliteStorage : IElysiumStorage
    {
        private readonly ElysiumSqliteStorageSettings _settings;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly string _connectionString;
        private readonly ConcurrentDictionary<string, TableDescriptor> _existingTables = [];
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
            LoadExistingTables();
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

        private void EnableWalMode(SqliteConnection connection)
        {
            using var command = connection.CreateCommand();
            command.CommandText = "PRAGMA journal_mode=WAL;";
            command.ExecuteNonQuery();
        }

        private void LoadExistingTables()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            EnableWalMode(connection);
            var query = "SELECT name FROM sqlite_master WHERE type = 'table'";
            using var command = new SqliteCommand(query, connection);
            using var reader = command.ExecuteReader();
            while(reader.Read())
            {
                var tableName = reader.GetString(0);
                if (string.IsNullOrEmpty(tableName))
                    continue;

                var columnQuery = $"PRAGMA table_info({SanitizeTableName(tableName)})";
                using var columnCommand = new SqliteCommand(columnQuery, connection);
                using var columnReader = columnCommand.ExecuteReader();

                var columns = new List<string>();
                while (columnReader.Read())
                {
                    var columnName = columnReader.GetString(1);
                    columns.Add(columnName);
                }
                _existingTables[tableName] = new TableDescriptor
                {
                    OrderedColumns = columns,
                };
            }
        }

        // todo: double check semaphore this b
        private async Task<(string TableName, string SanitizedTableName)> GetOrUpsertTableAsync(StorageKey storageKey, SqliteConnection connection, HashSet<string>? additionalColumns=null)
        {
            var tableName = SimpleTypeConverter.TypeToString(storageKey.Type);
            var sanitizedTableName = SanitizeTableName(tableName);
            //tableName = SanitizeTableName(tableName);

            HashSet<string>? columnsToAdd = null;
            var tableExists = _existingTables.ContainsKey(tableName);

            if (tableExists && additionalColumns != null)
            {
                columnsToAdd = new(additionalColumns);
                columnsToAdd.ExceptWith(_existingTables[tableName].Columns);
            }
            else
                columnsToAdd = additionalColumns;
            columnsToAdd?.Remove("Key");
            columnsToAdd?.Remove("KeyString");
            columnsToAdd?.Remove("Value");

            if (tableExists)
            {
                if (columnsToAdd == null || columnsToAdd.Count == 0)
                    return (tableName, sanitizedTableName);

                var tableDescriptor = _existingTables[tableName];

                foreach (var column in columnsToAdd)
                {
                    var alterTableQuery = $"ALTER TABLE {sanitizedTableName} ADD COLUMN {SanitizeTableName(column)} TEXT";
                    using var command = connection.CreateCommand();
                    command.CommandText = alterTableQuery;
                    await command.ExecuteNonQueryAsync();
                    _existingTables[tableName].OrderedColumns = _existingTables[tableName].OrderedColumns.Concat(columnsToAdd).ToList();
                }

                return (tableName, sanitizedTableName);
            }
            else
            {
                var columnList = new List<string>
                {
                    "Key",
                    "KeyString",
                    "Value"
                };
                if (columnsToAdd != null)
                    columnList.AddRange(columnsToAdd);

                var additionalColumnString = "";
                if (columnsToAdd != null && columnsToAdd.Count > 0)
                    additionalColumnString = string.Join("", columnsToAdd.Select(x => $",\n{SanitizeTableName(x)}"));


                var createTableQuery = $@"
                    CREATE TABLE IF NOT EXISTS {sanitizedTableName} (
                        Key TEXT PRIMARY KEY,
                        KeyString TEXT NOT NULL,
                        Value TEXT NOT NULL{additionalColumnString}
                    )";

                using var command = connection.CreateCommand();
                command.CommandText = createTableQuery;
                await command.ExecuteNonQueryAsync();

                _existingTables[tableName] = new TableDescriptor
                {
                    OrderedColumns = columnList
                };

                return (tableName, sanitizedTableName);
            }
        }

        public async Task<Result<T, StorageResultReason>> Get<T>(StorageKey<T> key)
        {
            var keyString = _storageKeyConverter.Serialize(key);
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            await EnableWalModeAsync(connection);
            var table = await GetOrUpsertTableAsync(key, connection);
            var query = $"SELECT Value FROM {table.SanitizedTableName} WHERE Key = @key";
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
            var table = await GetOrUpsertTableAsync(key, connection);
            string query = $"SELECT COUNT(1) FROM {table.SanitizedTableName} WHERE Key = @key";
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
            var table = await GetOrUpsertTableAsync(key, connection);
            string query = $"DELETE FROM {table.SanitizedTableName} WHERE Key = @key";
            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@key", keyString);
            var rowsAffected = await command.ExecuteNonQueryAsync();
            if (rowsAffected == 0)
                return new(StorageResultReason.NotFound);
            return new();
        }

        private string SanitizeTableName(string tableName)
        {
            // Escape any double quotes by replacing them with two double quotes
            var sanitized = tableName.Replace("\"", "\"\"");
    
            // Surround the sanitized table name with double quotes
            return $"\"{sanitized}\"";
        }

        public Task SetMany(List<(StorageKey Key, object Value)> values) => SetMany(values.Select(v => (v.Key, v.Value, (List<StorageKey>?)null)));

        public class KeyAndForeignKeyIdentity : IEquatable<KeyAndForeignKeyIdentity>
        {
            public required StorageKey PrimaryKey { get; set; }
            public required List<StorageKey> ForeignKeys { get; set; }


            public bool Equals(KeyAndForeignKeyIdentity? other)
            {
                if (other == null)
                    return false;
                return PrimaryKey == other.PrimaryKey && ForeignKeys.SequenceEqual(other.ForeignKeys);
            }

            public override int GetHashCode()
            {
                HashCode hashCode = default(HashCode);
                hashCode.Add(PrimaryKey.GetHashCode());

                foreach (var foreignKey in ForeignKeys)
                    hashCode.Add(foreignKey.GetHashCode());

                return hashCode.ToHashCode();
            }

            public override bool Equals(object? obj)
            {
                if (obj is KeyAndForeignKeyIdentity id)
                    return Equals(id);
                return false;
            }
        }

        public class SetDetails
        {
            public required StorageKey PrimaryKey { get; set; }
            public required object Value { get; set; }  
            public required List<StorageKey> ForeignKeys { get; set; }
        }

        public async Task SetMany(IEnumerable<(StorageKey Key, object Value, List<StorageKey>? ForeignKeys)> values)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            await EnableWalModeAsync(connection);
            using var transaction = await connection.BeginTransactionAsync();

            var details = values.Select(v => new SetDetails
            {
                ForeignKeys = v.ForeignKeys?.Distinct().Order().ToList() ?? [],
                PrimaryKey = v.Key,
                Value = v.Value
            });
            var foreignKeysGroupedByTable = details.GroupBy(d => d.PrimaryKey.Type)
                .ToDictionary(grp => grp.First().PrimaryKey, grp => grp.SelectMany(x => x.ForeignKeys));
            var tables = await Task.WhenAll(foreignKeysGroupedByTable.Select(kvp => GetOrUpsertTableAsync(kvp.Key, connection, kvp.Value.Select(_storageKeyConverter.GetTypeString).ToHashSet())));
            var keyedTables = foreignKeysGroupedByTable.Zip(tables).ToDictionary(x => x.First.Key.Type, x => x.Second);

            var valuesGroupedByKeyAndForeignKeys = details.Select(d => new KeyValuePair<KeyAndForeignKeyIdentity, SetDetails>(new KeyAndForeignKeyIdentity
            {
                PrimaryKey = d.PrimaryKey,
                ForeignKeys = d.ForeignKeys
            }, d)).GroupBy(kvp => kvp.Key).ToDictionary(grp => grp.Key, grp => grp.Select(x => x.Value).ToList());


            foreach(var (identity, setDetails) in valuesGroupedByKeyAndForeignKeys)
            {
                var (table, sanitizedTable) = keyedTables[identity.PrimaryKey.Type];
                var command = connection.CreateCommand();
                var parameters = new Dictionary<string, (string, Func<SetDetails, string>)>
                {
                    { "Key", ("key", x => _storageKeyConverter.Serialize(x.PrimaryKey)) },
                    { "Value", ("value", x => JsonConvert.SerializeObject(x.Value, _serializerSettings)) }
                };

                if (_settings.StoreKeyStrings)
                    parameters.Add("KeyString", ("keyString", x => x.PrimaryKey.ToString()));

                for(int i=0; i < identity.ForeignKeys.Count; i++)
                {
                    var foreignKey = identity.ForeignKeys[i];
                    var foreignKeyColumn = SanitizeTableName(_storageKeyConverter.GetTypeString(foreignKey));
                    var j = i; // this is necessary, something something functions
                    parameters.Add(foreignKeyColumn, ($"foreign_key_{j}", x => x.ForeignKeys[j].Parts[^1].Value));
                }

                command.CommandText = $"INSERT OR REPLACE INTO {sanitizedTable} ({string.Join(',', parameters.Keys)}) VALUES ({string.Join(',', parameters.Values.Select(v => $"@{v.Item1}"))})";


                var setParametersList = new List<Action<SetDetails>>();
                foreach (var set in parameters.Values)
                {
                    var sqliteParameter = command.CreateParameter();
                    sqliteParameter.ParameterName = $"@{set.Item1}";
                    command.Parameters.Add(sqliteParameter);
                    setParametersList.Add(vals => sqliteParameter.Value = set.Item2(vals));
                }

                foreach(var set in setDetails)
                {
                    foreach (var func in setParametersList)
                        func(set);
                    await command.ExecuteNonQueryAsync();
                }

            }


            //var commands = new Dictionary<Type, (SqliteCommand Command, Action<StorageKey, object> SetParameters)>();
            //async Task<(SqliteCommand Command, Action<StorageKey,  object> SetParameters)> PreparedCommandAsync(StorageKey storageKey, List<StorageKey>? foreignKeys)
            //{
            //    var table = await GetOrUpsertTableAsync(storageKey,connection, foreignKeys?.Select(_storageKeyConverter.GetTypeString).ToHashSet());

            //    var setParametersList = new List<Action<StorageKey,  object>>();

            //    var command = connection.CreateCommand();
            //    if (_settings.StoreKeyStrings)
            //       command.CommandText = $"INSERT OR REPLACE INTO {table} (Key, KeyString, Value) VALUES (@key, @keyString, @value)";
            //    else
            //       command.CommandText = $"INSERT OR REPLACE INTO {table} (Key, Value) VALUES (@key, @value)";

            //    var keyParameter = command.CreateParameter();
            //    keyParameter.ParameterName = "@key";
            //    command.Parameters.Add(keyParameter);
            //    setParametersList.Add((key, _) =>
            //    {
            //        keyParameter.Value = _storageKeyConverter.Serialize(key);
            //    });
            //    if (_settings.StoreKeyStrings)
            //    {
            //        var keyStringParameter = command.CreateParameter();
            //        keyStringParameter.ParameterName = "@keyString";
            //        command.Parameters.Add(keyStringParameter);
            //        setParametersList.Add((key, _) =>
            //        {
            //            keyStringParameter.Value = key.ToString();
            //        });
            //    }
            //    var valueParameter = command.CreateParameter();
            //    valueParameter.ParameterName = "@value";
            //    command.Parameters.Add(valueParameter);
            //    setParametersList.Add((_, value) =>
            //    {
            //        valueParameter.Value = JsonConvert.SerializeObject(value, _serializerSettings);
            //    });

            //    return (command, (key, value) =>
            //    {
            //        foreach (var action in setParametersList)
            //            action(key, value);
            //    });
            //}

            //foreach(var (key, value, foreignKeys) in values)
            //{
            //    var keyString = _storageKeyConverter.Serialize(key);
            //    var valueString = JsonConvert.SerializeObject(value, _serializerSettings);

            //    if (!commands.ContainsKey(key.Type))
            //        commands[key.Type] = await PreparedCommandAsync(key, foreignKeys);

            //    var (command, setParameters) = commands[key.Type];
            //    setParameters(key, value);
            //    await command.ExecuteNonQueryAsync();
            //}

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
                var table = await GetOrUpsertTableAsync(key, connection);
                var query = $"SELECT Value FROM {table.SanitizedTableName} WHERE Key = @key";
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

        public Task Set<T>(StorageKey<T> key, T value, List<StorageKey> foreignKeys)
        {
            return SetMany([(key, value!, foreignKeys)]);
        }

        public async Task<List<(StorageKey<TPrimary> Key, TPrimary Value)>> Get<TPrimary, TForeign>(StorageKey<TPrimary> partialPrimaryKey, StorageKey<TForeign> foreignKey)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            await EnableWalModeAsync(connection);

            var (table, sanitizedTable) = await GetOrUpsertTableAsync(partialPrimaryKey, connection);
            var tableDescriptor = _existingTables[table];
            var foreignKeyString = _storageKeyConverter.GetTypeString(foreignKey);
            if (!tableDescriptor.Columns.Contains(foreignKeyString))
                return [];


            var query = $"SELECT Key, Value FROM {sanitizedTable} WHERE {SanitizeTableName(foreignKeyString)} = @foreignValue";
            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@foreignValue", foreignKey.Parts[^1].Value);
            var reader = await command.ExecuteReaderAsync();


            var results = new List<(StorageKey<TPrimary>, TPrimary)>();
            while (reader.Read())
            {
                var key = reader.GetString(0);
                var value = reader.GetString(1);
                var obj = JsonConvert.DeserializeObject<TPrimary>(value)
                    ?? throw new JsonException("unable to deserialize result");
                var sk = _storageKeyConverter.Deserialize<TPrimary>(key)
                    ?? throw new JsonException("unable to deserialize key");
                results.Add((sk, obj));
            }

            return results;
        }

        public async Task<Result<int, StorageResultReason>> DeleteMany<TPrimary, TForegin>(StorageKey<TPrimary> partialPrimaryKey, StorageKey<TForegin> foreignKey)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            await EnableWalModeAsync(connection);

            var (table, sanitizedTable) = await GetOrUpsertTableAsync(partialPrimaryKey, connection);
            var tableDescriptor = _existingTables[table];
            var foreignKeyString = _storageKeyConverter.GetTypeString(foreignKey);

            if (!tableDescriptor.Columns.Contains(foreignKeyString))
                return new(StorageResultReason.NotFound);

            var query = $"DELETE FROM {sanitizedTable} WHERE {SanitizeTableName(foreignKeyString)} = @foreignValue";
            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@foreignValue", foreignKey.Parts[^1].Value);

            var rowsAffected = await command.ExecuteNonQueryAsync();

            if (rowsAffected == 0)
                return new(StorageResultReason.NotFound);

            return new(rowsAffected);
        }
    }
}

