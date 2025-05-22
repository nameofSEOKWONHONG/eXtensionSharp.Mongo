namespace eXtensionSharp.Mongo;

public interface IJMongoFactory
{
    JMongo<T> Create<T>() where T : class;
}