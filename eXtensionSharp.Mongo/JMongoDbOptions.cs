using MongoDB.Driver;

namespace eXtensionSharp.Mongo;

public class JMongoDbOptions
{
    internal List<Func<IServiceProvider, Task>> LambdaInitializers { get; } = new();
    internal List<Type> InitializerTypes { get; } = new();

    public void AddInitializer(Func<IServiceProvider, Task> initializer)
    {
        LambdaInitializers.Add(initializer);
    }

    public void AddInitializer<T>() where T : class, IJMongoIndexInitializer
    {
        InitializerTypes.Add(typeof(T));
    }
}