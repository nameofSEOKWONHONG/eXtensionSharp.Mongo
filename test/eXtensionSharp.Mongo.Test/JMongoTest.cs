using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace eXtensionSharp.Mongo.Test;

public class Tests
{
    private readonly string _mongoConnectionString = "mongodb://localhost:27017";

    
    [Test]
    public void jmongo_test()
    {
        var client = new MongoClient(_mongoConnectionString);
        var collection = JMongo<Sample>.Create(client).GetCollection();
        var item = new Sample { Name = "test", Age = 100 };
        collection.InsertOne(item);
        Assert.That(item.Id, Is.Not.Null);
    }
}

[JMongoCollection("sample", "demo")]
public class Sample
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("id")]
    public String Id { get; set; }
    
    [BsonElement("name")]
    public string Name { get; set; }
    
    [BsonElement("age")]
    public int Age { get; set; }
}