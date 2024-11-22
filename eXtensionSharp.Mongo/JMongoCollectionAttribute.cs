namespace eXtensionSharp.Mongo;

/// <summary>
/// Specifies metadata for a MongoDB collection, including the database name and collection name.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class JMongoCollectionAttribute : Attribute
{
    /// <summary>
    /// Gets the name of the database where the collection is stored.
    /// </summary>
    public string Database { get; }
    
    /// <summary>
    /// Gets the name of the collection in the specified database.
    /// </summary>
    public string Collection { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="JMongoCollectionAttribute"/> class with the specified database and collection names.
    /// </summary>
    /// <param name="database">The name of the database where the collection resides.</param>
    /// <param name="collection">The name of the collection in the database.</param>
    public JMongoCollectionAttribute(string database, string collection)
    {
        Database = database;
        Collection = collection;
    }
}