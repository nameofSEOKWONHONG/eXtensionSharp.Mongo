using MongoDB.Driver;

namespace eXtensionSharp.Mongo;

public sealed class JMongoFactory : IJMongoFactory, IJMongoFactoryBuilder
{
    private readonly IMongoClient _client;
    private readonly Dictionary<Type, object> _builders = new();

    public JMongoFactory(IMongoClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public void RegisterBuilder<T>(JMongoBuilder<T> builder) where T : class
    {
        _builders[typeof(T)] = builder ?? throw new ArgumentNullException(nameof(builder));
    }

    public JMongo<T> Create<T>() where T : class
    {
        if (!_builders.TryGetValue(typeof(T), out var obj) || obj is not JMongoBuilder<T> builder)
            throw new InvalidOperationException($"No JMongoBuilder registered for type '{typeof(T).Name}'.");

        return new JMongo<T>(builder.DatabaseName, builder.CollectionName, _client);
    }
}