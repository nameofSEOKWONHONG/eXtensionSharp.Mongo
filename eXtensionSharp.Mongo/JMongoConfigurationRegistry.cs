using MongoDB.Driver;

namespace eXtensionSharp.Mongo;

public class JMongoConfigurationRegistry
{
    internal List<Action<IMongoClient, IJMongoFactoryBuilder>> Executors { get; } = new();

    public void ApplyConfiguration<T>(IJMongoConfiguration<T> config) where T : class
    {
        Executors.Add((client, factory) =>
        {
            var builder = new JMongoCollectionBuilder<T>();
            config.Configure(builder);

            var collection = client
                .GetDatabase(builder.DatabaseName)
                .GetCollection<T>(builder.CollectionName);

            builder.ApplyIndexes(collection);
            factory.RegisterBuilder(builder);
        });
    }
}