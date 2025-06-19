using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace eXtensionSharp.Mongo.Test;

public class SampleTest : IClassFixture<MongoTestFixture>
{
    private readonly IJMongoFactory _factory;

    public SampleTest(MongoTestFixture fixture)
    {
        _factory = fixture.ServiceProvider.GetRequiredService<IJMongoFactory>();
    }

    [Fact]
    public async Task Test_CreateUser()
    {
        var collection = _factory.Create<SampleDocument>();
        var newItem = new SampleDocument
        {
            Name = "test2",
            Age = 200,
            CreatedAt = DateTimeOffset.Now,
        };

        await collection.InsertOneAsync(newItem);

        var count = await collection.CountDocumentsAsync(_ => true);
        Assert.Equal(1, count);
    }
}