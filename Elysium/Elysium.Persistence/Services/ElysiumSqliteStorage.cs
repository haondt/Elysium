using Elysium.Core.Converters;
using Elysium.Persistence.Converters;
using Haondt.Core.Models;
using Haondt.Identity.StorageKey;
using Haondt.Persistence.Services;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Elysium.Persistence.Services
{
    //public class TableDescriptor
    //{
    //    //private HashSet<string> _columns = [];
    //    //private List<string> _orderedColumns = [];
    //    //public HashSet<string> Columns { get { return _columns; } }
    //    //public List<string> OrderedColumns
    //    //{
    //    //    get {  return _orderedColumns; }
    //    //    set
    //    //    {
    //    //        _orderedColumns = value;
    //    //        _columns = new (value);
    //    //    }
    //    //}
    //}

    public class ElysiumSqliteStorage : IElysiumStorage
    {
        private readonly ElysiumSqliteStorageSettings _settings;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly string _connectionString;
        private readonly string _primaryTableName;
        private readonly string _foreignKeyTableName;

        //private readonly ConcurrentDictionary<string, TableDescriptor> _existingTables = [];

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
            //LoadExistingTables();
            _primaryTableName = SanitizeTableName(_settings.PrimaryTableName);
            _foreignKeyTableName = SanitizeTableName(_settings.ForeignKeyTableName);
            InitializeDb();
        }

        private JsonSerializerSettings ConfigureSerializerSettings(JsonSerializerSettings settings)
        {
            settings.TypeNameHandling = TypeNameHandling.Auto;
            settings.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Error;
            settings.Formatting = Newtonsoft.Json.Formatting.None;
            settings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            settings.Converters.Add(new GenericStorageKeyJsonConverter());
            return settings;
        }

        private string SanitizeTableName(string tableName)
        {
            // Escape any double quotes by replacing them with two double quotes
            var sanitized = tableName.Replace("\"", "\"\"");

            // Surround the sanitized table name with double quotes
            return $"\"{sanitized}\"";
        }

        private SqliteConnection GetConnection()
        {
            var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var walCommand = connection.CreateCommand();
            walCommand.CommandText = "PRAGMA journal_mode=WAL;";
            walCommand.ExecuteNonQuery();

            using var enableForeignKeysCommand = new SqliteCommand("PRAGMA foreign_keys = ON;", connection);
            enableForeignKeysCommand.ExecuteNonQuery();

            return connection;
        }

        private void WithConnection(Action<SqliteConnection> action)
        {
            using var connection = GetConnection();
            action(connection);
        }

        private T WithConnection<T>(Func<SqliteConnection, T> action)
        {
            using var connection = GetConnection();
            return action(connection);
        }

        private void WithTransaction(Action<SqliteConnection, SqliteTransaction> action)
        {
            using var connection = GetConnection();
            using var transaction = connection.BeginTransaction();
            try
            {
                action(connection, transaction);
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        private T WithTransaction<T>(Func<SqliteConnection, T> action)
        {
            using var connection = GetConnection();
            using var transaction = connection.BeginTransaction();
            try
            {
                var result = action(connection);
                transaction.Commit();
                return result;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        private void InitializeDb()
        {

            if (WithConnection(connection =>
            {
                var checkTableQuery = $"SELECT name FROM sqlite_master WHERE type = 'table' AND name = {_primaryTableName};";
                using var checkTableCommand = new SqliteCommand(checkTableQuery, connection);
                return checkTableCommand.ExecuteScalar() != null;
            }))
                return;

            WithTransaction((connection, transaaction) =>
            {
                using var createPrimaryTableCommand = new SqliteCommand(
                    $@"
                        CREATE TABLE {_primaryTableName} (
                        PrimaryKey TEXT PRIMARY KEY,
                        KeyString TEXT NOT NULL,
                        Value TEXT NOT NULL
                     );", connection, transaaction);
                createPrimaryTableCommand.ExecuteNonQuery();

                using var createForeignKeyTableCommand = new SqliteCommand(
                    $@"
                        CREATE TABLE {_foreignKeyTableName} (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        ForeignKey TEXT,
                        KeyString TEXT NOT NULL,
                        PrimaryKey TEXT,
                        FOREIGN KEY (PrimaryKey) REFERENCES {_primaryTableName}(PrimaryKey) ON DELETE CASCADE
                     );", connection, transaaction);
                createForeignKeyTableCommand.ExecuteNonQuery();
            });
        }

        public Task<Result<T, StorageResultReason>> Get<T>(StorageKey<T> key)
        {
            var keyString = StorageKeyConvert.Serialize(key);
            var result = WithConnection(connection =>
            {
                var query = $"SELECT Value FROM {_primaryTableName} WHERE PrimaryKey = @key";
                using var command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@key", keyString);
                return command.ExecuteScalar();
            });

            if (result == null)
                return Task.FromResult(new Result<T, StorageResultReason>(StorageResultReason.NotFound));
            var resultString = result.ToString()
                ?? throw new JsonException("unable to deserialize result");
            var value = JsonConvert.DeserializeObject<T>(resultString, _serializerSettings)
                ?? throw new JsonException("unable to deserialize result");
            return Task.FromResult(new Result<T, StorageResultReason>(value));
        }

        public Task<bool> ContainsKey(StorageKey key)
        {
            var keyString = StorageKeyConvert.Serialize(key);
            var count = WithConnection(connection =>
            {
                string query = $"SELECT COUNT(1) FROM {_primaryTableName} WHERE PrimaryKey = @key";
                using var command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@key", keyString);
                return command.ExecuteScalar();
            });
            if (count is not long longCount)
                throw new JsonException("unable to deserialize result");
            return Task.FromResult(longCount > 0);
        }

        public Task Set<T>(StorageKey<T> key, T value)
        {
            return Set(key, value, []);
        }

        public Task<Result<StorageResultReason>> Delete(StorageKey key)
        {
            var keyString = StorageKeyConvert.Serialize(key);
            var rowsAffected = WithConnection(connection =>
            {
                string query = $"DELETE FROM {_primaryTableName} WHERE PrimaryKey = @key";
                using var command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@key", keyString);
                return command.ExecuteNonQuery();
            });
            if (rowsAffected == 0)
                return Task.FromResult(new Result<StorageResultReason>(StorageResultReason.NotFound));
            return Task.FromResult(new Result<StorageResultReason>());
        }

        public Task SetMany(List<(StorageKey Key, object? Value)> values)
        {
            InternalSetMany(values.Select(v => (v.Key, v.Value, new List<StorageKey>())));
            return Task.CompletedTask;
        }

        //public class KeyAndForeignKeyIdentity : IEquatable<KeyAndForeignKeyIdentity>
        //{
        //    public required StorageKey PrimaryKey { get; set; }
        //    public required List<StorageKey> ForeignKeys { get; set; }


        //    public bool Equals(KeyAndForeignKeyIdentity? other)
        //    {
        //        if (other == null)
        //            return false;
        //        return PrimaryKey == other.PrimaryKey && ForeignKeys.SequenceEqual(other.ForeignKeys);
        //    }

        //    public override int GetHashCode()
        //    {
        //        HashCode hashCode = default(HashCode);
        //        hashCode.Add(PrimaryKey.GetHashCode());

        //        foreach (var foreignKey in ForeignKeys)
        //            hashCode.Add(foreignKey.GetHashCode());

        //        return hashCode.ToHashCode();
        //    }

        //    public override bool Equals(object? obj)
        //    {
        //        if (obj is KeyAndForeignKeyIdentity id)
        //            return Equals(id);
        //        return false;
        //    }
        //}

        private void InternalSetMany(IEnumerable<(StorageKey Key, object? Value, List<StorageKey> ForeignKeys)> values)
        {
            WithTransaction((connection, transaction) =>
            {
                var primaryTableInsertCommand = connection.CreateCommand();
                primaryTableInsertCommand.Transaction = transaction;
                SqliteCommand? foreignKeyTableInsertCommand = null;
                var setForeignKeyTableInsertCommandParameters = new List<Action<StorageKey, StorageKey>>();
                //if (_settings.StoreKeyStrings)
                //    primaryKeyInsertCommand.CommandText = $"INSERT OR REPLACE INTO {_primaryTableName} (Key, KeyString, Value) VALUES (@key, @keyString, @value)";
                //else
                //    primaryKeyInsertCommand.CommandText = $"INSERT OR REPLACE INTO {_primaryTableName} (Key, Value) VALUES (@key, @value)";

                var primaryTableInsertParameters = new List<(string Column, string ParameterName, Func<StorageKey, object?, List<StorageKey>, string> ParameterRetriever)>
                {
                    ("PrimaryKey", "primaryKey", (k, v, fk) => StorageKeyConvert.Serialize(k)),
                    ("Value", "value", (k, v, fk) => JsonConvert.SerializeObject(v, _serializerSettings))
                };
                if (_settings.StoreKeyStrings)
                    primaryTableInsertParameters.Add(("KeyString", "keyString", (k, v, fk) => k.ToString()));

                var setPrimaryTableInsertCommandParameters = new List<Action<StorageKey, object?, List<StorageKey>>>();
                foreach (var (column, parameterName, parameterRetriever) in primaryTableInsertParameters)
                {
                    var parameter = primaryTableInsertCommand.CreateParameter();
                    parameter.ParameterName = $"@{parameterName}";
                    primaryTableInsertCommand.Parameters.Add(parameter);
                    setPrimaryTableInsertCommandParameters.Add((k, v, pk) => parameter.Value = parameterRetriever(k, v, pk));
                }
                primaryTableInsertCommand.CommandText = $"INSERT OR REPLACE INTO {_primaryTableName} ({string.Join(',', primaryTableInsertParameters.Select(p => p.Column))})"
                    + $" VALUES ({string.Join(',', primaryTableInsertParameters.Select(q => $"@{q.ParameterName}"))})";

                var foreignKeyTableInsertParameters = new List<(string Column, string ParameterName, Func<StorageKey, StorageKey, string> ParameterRetriever)>
                {
                    ("ForeignKey", "foreignKey", (fk, pk) => StorageKeyConvert.Serialize(fk)),
                    ("PrimaryKey", "primaryKey", (fk, pk) => StorageKeyConvert.Serialize(pk)),
                };
                if (_settings.StoreKeyStrings)
                    foreignKeyTableInsertParameters.Add(("KeyString", "keyString", (fk, pk) => fk.ToString()));

                foreach (var (key, value, foreignKeys) in values)
                {

                    foreach (var setFunc in setPrimaryTableInsertCommandParameters)
                        setFunc(key, value, foreignKeys);

                    primaryTableInsertCommand.ExecuteNonQuery();

                    foreach (var foreignKey in foreignKeys)
                    {
                        if (foreignKeyTableInsertCommand == null)
                        {
                            foreignKeyTableInsertCommand = connection.CreateCommand();
                            foreignKeyTableInsertCommand.Transaction = transaction;
                            foreach (var (column, parameterName, parameterRetriever) in foreignKeyTableInsertParameters)
                            {
                                var parameter = foreignKeyTableInsertCommand.CreateParameter();
                                parameter.ParameterName = $"@{parameterName}";
                                foreignKeyTableInsertCommand.Parameters.Add(parameter);
                                setForeignKeyTableInsertCommandParameters.Add((fk, pk) => parameter.Value = parameterRetriever(fk, pk));
                            }
                            foreignKeyTableInsertCommand.CommandText = $"INSERT INTO {_foreignKeyTableName} ({string.Join(',', foreignKeyTableInsertParameters.Select(p => p.Column))})"
                                + $" VALUES ({string.Join(',', foreignKeyTableInsertParameters.Select(q => $"@{q.ParameterName}"))})";
                        }

                        foreach (var setFunc in setForeignKeyTableInsertCommandParameters)
                            setFunc(foreignKey, key);

                        foreignKeyTableInsertCommand.ExecuteNonQuery();
                    }
                }
            });

            //var foreignKeys = values.SelectMany(v => v.ForeignKeys)
            //    .Select(fk => fk.)

            //var details = values.Select(v => new SetDetails
            //{
            //    ForeignKeys = v.ForeignKeys?.Distinct().Order().ToList() ?? [],
            //    PrimaryKey = v.Key,
            //    Value = v.Value
            //});
            //var foreignKeysGroupedByTable = details.GroupBy(d => d.PrimaryKey.Type)
            //    .ToDictionary(grp => grp.First().PrimaryKey, grp => grp.SelectMany(x => x.ForeignKeys));
            //var tables = await Task.WhenAll(foreignKeysGroupedByTable.Select(kvp => GetOrUpsertTableAsync(connection, kvp.Value.Select(k => StorageKeyConvert.Serialize(k.WithoutFinalValue())).ToHashSet())));
            //var keyedTables = foreignKeysGroupedByTable.Zip(tables).ToDictionary(x => x.First.Key.Type, x => x.Second);

            //var valuesGroupedByKeyAndForeignKeys = details.Select(d => new KeyValuePair<KeyAndForeignKeyIdentity, SetDetails>(new KeyAndForeignKeyIdentity
            //{
            //    PrimaryKey = d.PrimaryKey,
            //    ForeignKeys = d.ForeignKeys
            //}, d)).GroupBy(kvp => kvp.Key).ToDictionary(grp => grp.Key, grp => grp.Select(x => x.Value).ToList());


            //foreach(var (identity, setDetails) in valuesGroupedByKeyAndForeignKeys)
            //{
            //    var (table, sanitizedTable) = keyedTables[identity.PrimaryKey.Type];
            //    var command = connection.CreateCommand();
            //    var parameters = new Dictionary<string, (string, Func<SetDetails, string>)>
            //    {
            //        { "Key", ("key", x => StorageKeyConvert.Serialize(x.PrimaryKey)) },
            //        { "Value", ("value", x => JsonConvert.SerializeObject(x.Value, _serializerSettings)) }
            //    };

            //    if (_settings.StoreKeyStrings)
            //        parameters.Add("KeyString", ("keyString", x => x.PrimaryKey.ToString()));

            //    for(int i=0; i < identity.ForeignKeys.Count; i++)
            //    {
            //        var foreignKey = identity.ForeignKeys[i];
            //        var foreignKeyColumn = SanitizeTableName(StorageKeyConvert.Serialize(foreignKey.WithoutFinalValue()));
            //        var j = i; // this is necessary, something something functions
            //        parameters.Add(foreignKeyColumn, ($"foreign_key_{j}", x => x.ForeignKeys[j].Parts[^1].Value));
            //    }

            //    command.CommandText = $"INSERT OR REPLACE INTO {sanitizedTable} ({string.Join(',', parameters.Keys)}) VALUES ({string.Join(',', parameters.Values.Select(v => $"@{v.Item1}"))})";


            //    var setParametersList = new List<Action<SetDetails>>();
            //    foreach (var set in parameters.Values)
            //    {
            //        var sqliteParameter = command.CreateParameter();
            //        sqliteParameter.ParameterName = $"@{set.Item1}";
            //        command.Parameters.Add(sqliteParameter);
            //        setParametersList.Add(vals => sqliteParameter.Value = set.Item2(vals));
            //    }

            //    foreach(var set in setDetails)
            //    {
            //        foreach (var func in setParametersList)
            //            func(set);
            //        await command.ExecuteNonQueryAsync();
            //    }

        }


        //var commands = new Dictionary<Type, (SqliteCommand Command, Action<StorageKey, object> SetParameters)>();
        //async Task<(SqliteCommand Command, Action<StorageKey,  object> SetParameters)> PreparedCommandAsync(StorageKey storageKey, List<StorageKey>? foreignKeys)
        //{
        //    var table = await GetOrUpsertTableAsync(storageKey,connection, foreignKeys?.Select(ElysiumStorageKeyConvert.SerializeWithoutLastValue).ToHashSet());

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
        //        keyParameter.Value = StorageKeyConvert.Serialize(key);
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
        //    var keyString = StorageKeyConvert.Serialize(key);
        //    var valueString = JsonConvert.SerializeObject(value, _serializerSettings);

        //    if (!commands.ContainsKey(key.Type))
        //        commands[key.Type] = await PreparedCommandAsync(key, foreignKeys);

        //    var (command, setParameters) = commands[key.Type];
        //    setParameters(key, value);
        //    await command.ExecuteNonQueryAsync();
        //}

        //    await transaction.CommitAsync();
        //}

        public Task<List<(StorageKey<T> Key, T Value)>> GetMany<T>(StorageKey<T> foreignKey)
        {
            var keyString = StorageKeyConvert.Serialize(foreignKey);
            var results = WithConnection(connection =>
            {
                var query = $@"
                    SELECT {_primaryTableName}.PrimaryKey, {_primaryTableName}.Value
                    FROM {_foreignKeyTableName}
                    JOIN {_primaryTableName} ON {_foreignKeyTableName}.PrimaryKey = {_primaryTableName}.PrimaryKey
                    WHERE {_foreignKeyTableName}.ForeignKey = @key
                ";
                using var command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@key", keyString);

                var reader = command.ExecuteReader();

                var results = new List<(StorageKey<T>, T)>();
                while (reader.Read())
                {
                    var primaryKeyString = reader.GetString(0);
                    var valueString = reader.GetString(1);

                    var value = JsonConvert.DeserializeObject<T>(valueString, _serializerSettings)
                        ?? throw new JsonException("unable to deserialize result");
                    var primaryKey = StorageKeyConvert.Deserialize<T>(primaryKeyString)
                        ?? throw new JsonException("unable to deserialize key");
                    results.Add((primaryKey, value));
                }

                return results;
            });

            return Task.FromResult(results);
        }

        public Task Set<T>(StorageKey<T> key, T value, List<StorageKey<T>> foreignKeys)
        {
            InternalSetMany([(key, value!, foreignKeys.Cast<StorageKey>().ToList())]);
            return Task.CompletedTask;
        }

        public Task<Result<int, StorageResultReason>> DeleteMany<T>(StorageKey<T> foreignKey)
        {
            var keyString = StorageKeyConvert.Serialize(foreignKey);

            var rowsAffected = WithConnection(connection =>
            {
                var query = $@"
                    DELETE FROM {_primaryTableName}
                    WHERE EXISTS (
                        SELECT 1
                        FROM {_foreignKeyTableName}
                        WHERE {_foreignKeyTableName}.PrimaryKey = {_primaryTableName}.PrimaryKey
                        AND {_foreignKeyTableName}.ForeignKey = @key
                    );
                ";

                using var command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@key", keyString);
                return command.ExecuteNonQuery();
            });

            if (rowsAffected == 0)
                return Task.FromResult(new Result<int, StorageResultReason>(StorageResultReason.NotFound));
            return Task.FromResult(new Result<int, StorageResultReason>(rowsAffected));
        }

        public async Task<List<Result<T, StorageResultReason>>> GetMany<T>(List<StorageKey<T>> keys)
        {
            var results = await GetMany(keys.Cast<StorageKey>().ToList());
            return results.Select(r =>
            {
                if (r.IsSuccessful)
                    return new(TypeCoercer.Coerce<T>(r.Value));
                return new Result<T, StorageResultReason>(r.Reason);
            }).ToList();
        }

        public Task<List<Result<object?, StorageResultReason>>> GetMany(List<StorageKey> keys)
        {
            var results = WithConnection(connection =>
            {
                var query = $@"
                    SELECT Value
                    FROM {_primaryTableName}
                    WHERE PrimaryKey = @key;
                ";
                using var command = new SqliteCommand(query, connection);
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@key";
                command.Parameters.Add(parameter);

                var results = new List<Result<object?, StorageResultReason>>();
                foreach (var key in keys)
                {
                    var keyString = StorageKeyConvert.Serialize(key);
                    parameter.Value = keyString;
                    var result = command.ExecuteScalar();
                    if (result == null)
                    {
                        results.Add(new(StorageResultReason.NotFound));
                        continue;
                    }
                    var resultString = result.ToString()
                        ?? throw new JsonException("unable to deserialize result");
                    var value = JsonConvert.DeserializeObject(resultString, key.Type, _serializerSettings)
                        ?? throw new JsonException("unable to deserialize result");
                    results.Add(new(value));
                }

                return results;
            });

            return Task.FromResult(results);
        }
    }
}

