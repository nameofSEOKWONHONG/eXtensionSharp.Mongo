namespace eXtensionSharp.Mongo;

[AttributeUsage(AttributeTargets.Class)]
public class JMongoCollectionAttribute : Attribute
{
    public string DatabaseName { get; }
    public string CollectionName { get; }

    public JMongoCollectionAttribute(string databaseName, string collectionName)
    {
        DatabaseName = string.IsNullOrWhiteSpace(databaseName) 
            ? throw new ArgumentException("Database name must not be null or empty.", nameof(databaseName)) 
            : databaseName;

        CollectionName = string.IsNullOrWhiteSpace(collectionName) 
            ? throw new ArgumentException("Collection name must not be null or empty.", nameof(collectionName)) 
            : collectionName;
    }
}