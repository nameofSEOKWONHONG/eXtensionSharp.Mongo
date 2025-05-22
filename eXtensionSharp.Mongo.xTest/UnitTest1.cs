using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace eXtensionSharp.Mongo.xTest;

public class UnitTest1 : IClassFixture<JMongoIntegrationTestContext>
{
    private readonly IJMongoFactory _factory;

    public UnitTest1(JMongoIntegrationTestContext context)
    {
        _factory = context.Provider.GetRequiredService<IJMongoFactory>();
    }

    [Fact]
    public void Create_ShouldReturnJMongo()
    {
        var collection = _factory.Create<SampleDocument>().GetCollection();
        Assert.NotNull(collection);
    }

    [Fact]
    public void Create_ShouldReturnJMongo_With_Custom_CollectionName()
    {
        var collection = _factory.Create<SampleDocument>().GetCollection();
        Assert.NotNull(collection);
    }

    [Fact]
    public void Create_ShouldReturnJMongo_With_Custom_DatabaseName()
    {
        var collection = _factory.Create<SampleDocument>().GetCollection();
        Assert.NotNull(collection);
    }

    [Fact]
    public void Create_SampleDocument()
    {
        var collection = _factory.Create<SampleDocument>().GetCollection();
        var newItem = new SampleDocument { Name = "test", Age = 100 };
        collection.InsertOne(newItem);
        Assert.NotEmpty(newItem.Id);
    }

    [Fact]
    public void Create_Sample2Document()
    {
        var collection = _factory.Create<SampleDocument2>().GetCollection();
        var newItem = new SampleDocument2 { Name = "test", Age = 100 };
        collection.InsertOne(newItem);
        Assert.NotEmpty(newItem.Id);
    }


    [JMongoCollection("sample", "demo")]
    public class SampleDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("id")]
        public String Id { get; set; }

        [BsonElement("name")] public string Name { get; set; }

        [BsonElement("age")] public int Age { get; set; }
    }

    [JMongoCollection("sample2", "demo")]
    public class SampleDocument2
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("id")]
        public String Id { get; set; }

        [BsonElement("name")] public string Name { get; set; }

        [BsonElement("age")] public int Age { get; set; }
    }
}