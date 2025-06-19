namespace eXtensionSharp.Mongo;

public interface IJMongoFactoryBuilder
{
    void RegisterBuilder<T>(JMongoBuilder<T> builder) where T : class;
    bool TryGetBuilder<T>(out JMongoBuilder<T> builder) where T : class;

    IReadOnlyDictionary<Type, object> GetAllBuilders();
}