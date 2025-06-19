using MongoDB.Driver;

namespace eXtensionSharp.Mongo;

public interface IJMongoFactory
{
    IMongoCollection<T> Create<T>() where T : class;
}

