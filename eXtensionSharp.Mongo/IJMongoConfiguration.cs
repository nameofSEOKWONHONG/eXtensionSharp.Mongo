namespace eXtensionSharp.Mongo;

public interface IJMongoConfiguration<T> where T : class
{
    void Configure(JMongoBuilder<T> builder);
}