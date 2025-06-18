using MongoDB.Driver;

namespace eXtensionSharp.Mongo;

public class JMongoDbOptions
{
    internal List<Action<IMongoClient, IJMongoFactoryBuilder>> Executors { get; } = new();

    public void ApplyConfiguration<T>(IJMongoConfiguration<T> config) where T : class
    {
        Executors.Add((client, factory) =>
        {
            var builder = new JMongoBuilder<T>();
            config.Configure(builder);

            var collection = client
                .GetDatabase(builder.DatabaseName)
                .GetCollection<T>(builder.CollectionName);

            builder.ApplyIndexes(collection);
            factory.RegisterBuilder(builder);
        });
    }
}