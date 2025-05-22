using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace eXtensionSharp.Mongo.TestConsole;

[JMongoCollection("sample", "demo")]
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

public class SampleDocumentIndexInitializer: IJMongoIndexInitializer
{
    public async Task InitializeIndexesAsync(IServiceProvider provider)
    {
        var factory = provider.GetRequiredService<IJMongoFactory>();
        var collection = factory.Create<SampleDocument>().GetCollection();
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<SampleDocument>(
                Builders<SampleDocument>.IndexKeys.Ascending(x => x.CreatedAt),
                new CreateIndexOptions { ExpireAfter = TimeSpan.FromDays(2) }
            )
        );
    }
}