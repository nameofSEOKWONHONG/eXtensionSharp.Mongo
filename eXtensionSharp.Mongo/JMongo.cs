using System.Reflection;
using MongoDB.Driver;

namespace eXtensionSharp.Mongo;

/// <summary>
/// A generic class that provides access to a MongoDB collection for a specified entity type.
/// The entity type must be annotated with the <see cref="JMongoCollectionAttribute"/> to define the database and collection names.
/// </summary>
/// <typeparam name="T">The entity type corresponding to the MongoDB collection. Must be a reference type.</typeparam>
public class JMongo<T>
    where T : class
{
    private JMongo(string databaseName, string collectionName, IMongoClient client)
    {
        _databaseName = databaseName;
        _collectionName = collectionName;
        _client = client;
    }

    /// <summary>
    /// The MongoDB client instance used to interact with the database.
    /// </summary>
    private IMongoClient _client;

    /// <summary>
    /// The name of the database where the collection resides.
    /// </summary>
    private string _databaseName;

    /// <summary>
    /// The name of the MongoDB collection.
    /// </summary>
    private string _collectionName;

    /// <summary>
    /// Retrieves the MongoDB collection for the specified entity type.
    /// </summary>
    /// <returns>
    /// An <see cref="IMongoCollection{T}"/> representing the MongoDB collection.
    /// </returns>
    public IMongoCollection<T> GetCollection()
    {
        var db = _client.GetDatabase(_databaseName);
        return db.GetCollection<T>(_collectionName);
    }

    /// <summary>
    /// Creates an instance of the <see cref="JMongo{T}"/> class for the specified entity type.
    /// </summary>
    /// <param name="client">The MongoDB client to be used for database access.</param>
    /// <returns>
    /// An instance of <see cref="JMongo{T}"/> configured to access the MongoDB collection for the entity type.
    /// </returns>
    /// <exception cref="Exception">
    /// Thrown if the entity type <typeparamref name="T"/> does not have the <see cref="JMongoCollectionAttribute"/> defined.
    /// </exception>
    public static JMongo<T> Create(IMongoClient client)
    {
        var attribute = typeof(T).GetCustomAttribute<JMongoCollectionAttribute>();
        if (attribute == null) throw new Exception("Not define MongoCollectionAttribute");
        return new JMongo<T>(attribute.Database, attribute.Collection, client);
    }
}
