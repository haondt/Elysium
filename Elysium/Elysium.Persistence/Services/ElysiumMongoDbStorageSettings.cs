namespace Elysium.Persistence.Services
{
    public class ElysiumMongoDbStorageSettings
    {
        public string Database { get; set; } = "elysium";
        public string Collection { get; set; } = "elysium";
        public bool StoreKeyStrings { get; set; } = false;
    }
}