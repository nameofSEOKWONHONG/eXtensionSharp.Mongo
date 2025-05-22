namespace eXtensionSharp.Mongo;

public interface IJMongoIndexInitializer
{
    Task InitializeIndexesAsync(IServiceProvider provider);
}