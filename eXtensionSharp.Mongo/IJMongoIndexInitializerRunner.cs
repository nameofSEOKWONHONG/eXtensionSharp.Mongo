using MongoDB.Driver;

namespace eXtensionSharp.Mongo;

public interface IJMongoIndexInitializerRunner
{
    Task RunAsync();
}
