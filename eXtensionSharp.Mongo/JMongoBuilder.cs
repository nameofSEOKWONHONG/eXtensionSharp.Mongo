using MongoDB.Driver;

namespace eXtensionSharp.Mongo;

public class JMongoBuilder<T> where T : class
{
    internal string DatabaseName { get; private set; }
    internal string CollectionName { get; private set; }

    private readonly List<Action<IMongoCollection<T>>> _indexActions = new();
    private bool _indexApplied = false;

    public JMongoBuilder<T> ToDocument(string dbName, string collectionName)
    {
        DatabaseName = dbName;
        CollectionName = collectionName;
        return this;
    }

    public JMongoBuilder<T> ToIndex(Action<IMongoCollection<T>> configureIndex)
    {
        _indexActions.Add(configureIndex);
        return this;
    }

    internal void ApplyIndexes(IMongoCollection<T> collection)
    {
        if (_indexApplied)
            return;

        foreach (var action in _indexActions)
        {
            action(collection);
        }

        _indexApplied = true;
    }
}