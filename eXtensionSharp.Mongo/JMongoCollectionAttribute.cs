namespace eXtensionSharp.Mongo;

[AttributeUsage(AttributeTargets.Class)]
public class JMongoCollectionAttribute : Attribute
{
    public string DatabaseName { get; }
    public string CollectionName { get; }

    public JMongoCollectionAttribute(string databaseName, string collectionName)
    {
        DatabaseName = databaseName;
        CollectionName = collectionName;
    }
}