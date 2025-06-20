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

/*
 * [문제점 고찰]
 * EF와 비슷한 형식으로 개발하고자 하였으나 IJMongoConfiguration이 문제임.
 * 여기서 CreateIndex는 Migration 개념이 아닌 프로그램 시작시 등록 동작이 발생함.
 * 따라서, 대량 데이터가 있을 경우 Index 생성을 즉시 진행함으로 문제가 될 수 있음.
 * 다만, Mongodb의 특정상 데이터 Migration을 사용하지 않으므로 신규로 Collection을 생성하는
 * 방법을 추천할 수 있음. (ex: V1, V2, V3...)
 * Mongodb가 데이터 백업 및 추출, 단기 보관이 되는 용도로 사용되므로 합리적이라 생각됨.
*/