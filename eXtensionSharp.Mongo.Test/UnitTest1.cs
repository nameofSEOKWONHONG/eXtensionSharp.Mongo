using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Moq;

namespace eXtensionSharp.Mongo.Test;

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
        services.AddSingleton<IJMongoFactory, JMongoCollectionFactory>();
        services.AddSingleton<IJMongoFactoryBuilder>(sp => (IJMongoFactoryBuilder)sp.GetRequiredService<IJMongoFactory>());

        var options = new JMongoConfigurationRegistry();
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
