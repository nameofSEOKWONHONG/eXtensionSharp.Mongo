namespace eXtensionSharp.Mongo;

public interface IJMongoConfiguration<T> where T : class
{
    void Configure(JMongoCollectionBuilder<T> collectionBuilder);
    string Description => $"{typeof(T).Name} collection configuration.";
}