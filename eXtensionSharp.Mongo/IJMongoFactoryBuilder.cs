namespace eXtensionSharp.Mongo;

public interface IJMongoFactoryBuilder
{
    void RegisterBuilder<T>(JMongoCollectionBuilder<T> collectionBuilder) where T : class;
    bool TryGetBuilder<T>(out JMongoCollectionBuilder<T> collectionBuilder) where T : class;

    IReadOnlyDictionary<Type, object> GetAllBuilders();
}