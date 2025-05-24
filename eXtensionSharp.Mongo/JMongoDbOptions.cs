using MongoDB.Driver;

namespace eXtensionSharp.Mongo;

public class JMongoDbOptions
{
    internal List<Type> InitializerTypes { get; } = new();

    public void AddInitializer<T>() where T : class, IJMongoConfiguration
    {
        InitializerTypes.Add(typeof(T));
    }
}