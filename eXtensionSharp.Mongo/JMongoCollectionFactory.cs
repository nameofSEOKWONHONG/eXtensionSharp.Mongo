using System.Collections.Concurrent;
using MongoDB.Driver;

namespace eXtensionSharp.Mongo;

public sealed class JMongoCollectionFactory : IJMongoFactory, IJMongoFactoryBuilder
{
    private readonly IMongoClient _client;
    private readonly ConcurrentDictionary<Type, object> _builders = new();

    public JMongoCollectionFactory(IMongoClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public void RegisterBuilder<T>(JMongoCollectionBuilder<T> collectionBuilder) where T : class
    {
        _builders[typeof(T)] = collectionBuilder ?? throw new ArgumentNullException(nameof(collectionBuilder));
    }
    
    public bool TryGetBuilder<T>(out JMongoCollectionBuilder<T> collectionBuilder) where T : class
    {
        if (_builders.TryGetValue(typeof(T), out var obj) && obj is JMongoCollectionBuilder<T> typed)
        {
            collectionBuilder = typed;
            return true;
        }

        collectionBuilder = null!;
        return false;
    }

    public IReadOnlyDictionary<Type, object> GetAllBuilders() => _builders;

    public IMongoCollection<T> Create<T>() where T : class
    {
        if (!_builders.TryGetValue(typeof(T), out var obj) || obj is not JMongoCollectionBuilder<T> builder)
            throw new InvalidOperationException($"No JMongoBuilder registered for type '{typeof(T).Name}'.");

        return new JMongo<T>(builder.DatabaseName, builder.CollectionName, _client).GetCollection();
    }
}