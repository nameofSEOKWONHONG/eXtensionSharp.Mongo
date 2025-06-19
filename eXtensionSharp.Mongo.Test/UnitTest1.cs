using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Moq;

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
    public void Configure(JMongoBuilder<SampleDocument> builder)
    {
        builder.ToDocument("sample", "demo");
        builder.ToIndex(indexes =>
        {
            indexes.CreateOne(new CreateIndexModel<SampleDocument>(
                Builders<SampleDocument>.IndexKeys.Ascending(x => x.CreatedAt),
                new CreateIndexOptions { ExpireAfter = TimeSpan.FromDays(1) }));
        });
    }
}

public class JMongoTests
{
    [Fact]
    public void ApplyConfiguration_RegistersBuilder_AndCreatesCollection()
    {
        var mockClient = new Mock<IMongoClient>();
        var mockDb = new Mock<IMongoDatabase>();
        var mockCollection = new Mock<IMongoCollection<SampleDocument>>();
        var mockIndexes = new Mock<IMongoIndexManager<SampleDocument>>();

        // Step 1: IndexManager 반환
        mockCollection
            .SetupGet(c => c.Indexes)
            .Returns(mockIndexes.Object);

        // Step 2: CreateOne 호출 허용
        mockIndexes
            .Setup(i => i.CreateOne(
                It.IsAny<CreateIndexModel<SampleDocument>>(),
                null,
                default))
            .Verifiable();  // 테스트에서 Verify 용도

        // Step 3: Collection 반환
        mockDb
            .Setup(d => d.GetCollection<SampleDocument>("demo", null))
            .Returns(mockCollection.Object);

        // Step 4: Database 반환
        mockClient
            .Setup(c => c.GetDatabase("sample", null))
            .Returns(mockDb.Object);

        var services = new ServiceCollection();
        services.AddSingleton<IMongoClient>(mockClient.Object);
        services.AddSingleton<IJMongoFactory, JMongoFactory>();
        services.AddSingleton<IJMongoFactoryBuilder>(sp => (IJMongoFactoryBuilder)sp.GetRequiredService<IJMongoFactory>());

        var options = new JMongoDbOptions();
        options.ApplyConfiguration(new SampleDocumentConfiguration());
        services.AddSingleton(options);

        var provider = services.BuildServiceProvider();

        using var scope = provider.CreateScope();
        var builder = new ApplicationBuilder(scope.ServiceProvider);

        // Act
        builder.UseJMongoDb(); // 이 메서드 내에서 GetDatabase 호출되어야 함

        // Assert
        mockClient.Verify(c => c.GetDatabase("sample", null), Times.Once);
        mockDb.Verify(d => d.GetCollection<SampleDocument>("demo", null), Times.Once);
    }
}
