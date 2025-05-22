using System.Collections.Concurrent;
using System.Reflection;
using MongoDB.Driver;

namespace eXtensionSharp.Mongo;

public sealed class JMongoFactory : IJMongoFactory
{
    private readonly IMongoClient _client;
    
    private static readonly ConcurrentDictionary<Type, JMongoCollectionAttribute> _attributeCache = new();


    public JMongoFactory(IMongoClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public JMongo<T> Create<T>() where T : class
    {
        var type = typeof(T);

        var attribute = _attributeCache.GetOrAdd(type, t =>
            t.GetCustomAttribute<JMongoCollectionAttribute>() 
            ?? throw new InvalidOperationException($"Type '{t.FullName}' must have [JMongoCollectionAttribute]."));

        return new JMongo<T>(attribute.DatabaseName, attribute.CollectionName, _client);
    }
}