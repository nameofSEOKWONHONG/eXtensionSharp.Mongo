using MongoDB.Driver;

namespace eXtensionSharp.Mongo;

/// <summary>
/// MongoDB 컬렉션에 접근하기 위한 제네릭 래퍼 클래스
/// </summary>
/// <typeparam name="T">도큐먼트 형식</typeparam>
public sealed class JMongo<T> where T : class
{
    private readonly IMongoClient _client;
    private readonly string _databaseName;
    private readonly string _collectionName;

    public JMongo(string databaseName, string collectionName, IMongoClient client)
    {
        _databaseName = databaseName ?? throw new ArgumentNullException(nameof(databaseName));
        _collectionName = collectionName ?? throw new ArgumentNullException(nameof(collectionName));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    /// <summary>
    /// 지정된 컬렉션을 반환합니다.
    /// </summary>
    public IMongoCollection<T> GetCollection()
    {
        return _client
            .GetDatabase(_databaseName)
            .GetCollection<T>(_collectionName);
    }
}