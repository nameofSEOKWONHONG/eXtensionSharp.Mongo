using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace eXtensionSharp.Mongo.Test;

// 테스트용 컬렉션 도메인 클래스
public class SampleDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("id")]
    public String Id { get; set; }

    [BsonElement("name")] public string Name { get; set; }

    [BsonElement("age")] public int Age { get; set; }
    [BsonElement("createdAt")] public DateTimeOffset CreatedAt { get; set; }
}

public class SampleDocumentConfiguration: IJMongoConfiguration<SampleDocument>
{
    public void Configure(JMongoCollectionBuilder<SampleDocument> collectionBuilder)
    {
        collectionBuilder.ToDocument("sample", "demo");
        collectionBuilder.ToIndex(indexes =>
        {
            indexes.CreateOne(new CreateIndexModel<SampleDocument>(
                Builders<SampleDocument>.IndexKeys.Ascending(x => x.CreatedAt),
                new CreateIndexOptions { ExpireAfter = TimeSpan.FromDays(1) }));
        });
    }
}