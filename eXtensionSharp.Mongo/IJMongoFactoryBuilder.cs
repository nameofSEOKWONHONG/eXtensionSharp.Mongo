namespace eXtensionSharp.Mongo;

public interface IJMongoFactoryBuilder
{
    void RegisterBuilder<T>(JMongoBuilder<T> builder) where T : class;
}