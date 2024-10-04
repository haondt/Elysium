namespace Elysium.Persistence.Services
{
    public class ElysiumPostgresqlStorageSettings
    {
        public required string Host { get; set; }
        public string Database { get; set; } = "elysium";
        public string Username { get; set; } = "elysium";
        public string Password { get; set; } = "elysium";
        public int Port { get; set; } = 5432;
        public string PrimaryTableName { get; set; } = "elysium";
        public string ForeignKeyTableName { get; set; } = "foreignKeys";
        public bool StoreKeyStrings { get; set; } = false;
    }
}