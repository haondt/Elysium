namespace Elysium.Persistence.Services
{
    public class ElysiumPersistenceSettings
    {
        public ElysiumPersistenceDrivers Driver { get; set; } = ElysiumPersistenceDrivers.Memory;
        public ElysiumSqliteStorageSettings? SqliteStorageSettings { get; set; }
        public ElysiumMongoDbStorageSettings? MongoDbStorageSettings { get; set; }
        public ElysiumPostgresqlStorageSettings? PostgresqlStorageSettings { get; set; }
    }
}
