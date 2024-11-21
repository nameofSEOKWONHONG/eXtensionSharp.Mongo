using System.Reflection;
using MongoDB.Driver;

namespace eXtensionSharp.Mongo;

public class JMongo<T>
    where T : class
{
    private JMongo(string databaseName, string collectionName, IMongoClient client)
    {
        _databaseName = databaseName;
        _collectionName = collectionName;
        _client = client;
    }
    
    private IMongoClient _client;
    private string _databaseName;
    private string _collectionName;

    public IMongoCollection<T> GetCollection()
    {
        var db = _client.GetDatabase(_databaseName);
        return db.GetCollection<T>(_collectionName);
    }

    public static JMongo<T> Create(IMongoClient client)
    {
        var attribute = typeof(T).GetCustomAttribute<JMongoCollectionAttribute>();
        if (attribute == null) throw new Exception("Not define MongoCollectionAttribute");
        return new JMongo<T>(attribute!.DatabaseName, attribute.CollectionName, client);
    }
}