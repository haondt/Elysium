namespace Elysium.Persistence.Services
{
    public class ElysiumSqliteStorageSettings
    {
        public string PrimaryTableName { get; set; } = "elysium";
        public string ForeignKeyTableName { get; set; } = "foreignKeys";
        public string DatabasePath { get; set; } = "./ElysiumData.db";
        public bool StoreKeyStrings { get; set; } = false;
    }
}
