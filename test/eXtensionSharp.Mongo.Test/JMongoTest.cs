using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace eXtensionSharp.Mongo.Test;

public class Tests
{
    private readonly string _mongoConnectionString = "mongodb://localhost:27017";
    private MongoClient _client;

    [SetUp]
    public void setup()
    {
        _client = new MongoClient(_mongoConnectionString);
    }
    
    [Test]
    public void jmongo_test()
    {
        var client = new MongoClient(_mongoConnectionString);
        var collection = JMongo<Sample>.Create(client).GetCollection();
        var item = new Sample { Name = "test", Age = 100 };
        collection.InsertOne(item);
        Assert.That(item.Id, Is.Not.Null);
    }

    [Test]
    public async Task jmongo_count_test()
    {
        var collection = JMongo<Sample>.Create(_client).GetCollection();
        var cnt = await collection.Find(new BsonDocument()).CountDocumentsAsync();
    }

    [Test]
    public async Task jmongo_get_test()
    {
        var collection = JMongo<Sample>.Create(_client).GetCollection();
        var item = await collection.AsQueryable().FirstOrDefaultAsync(m => m.Id == "673fd3a7fc2174ca893e1dbf");
        Assert.That(item, Is.Not.Null);
    }

    [Test]
    public async Task jmongo_batch_insert_test()
    {
        var samples = new List<Sample>();
         
        Enumerable.Range(1, 5).ToList().ForEach(i =>
        {
            var item = new Sample { Name = $"N_TEST", Age = i, CreatedBy = "TEST", CreatedOn = DateTime.UtcNow };
            samples.Add(item);
            Thread.Sleep(200);
        });
        
        var collection = JMongo<Sample>.Create(_client).GetCollection();
        await collection.InsertManyAsync(samples);
    }

    [Test]
    public async Task jmongo_select_test()
    {
        var collection = JMongo<Sample>.Create(_client).GetCollection();
        var date = DateTime.UtcNow;
        var from = date.xToFromDate();
        var to = date.xToToDate();
        var item = await collection.AsQueryable().AsQueryable().Where(m => m.CreatedOn >= from && m.CreatedOn < to).ToListAsync();
        Assert.That(item, Is.Not.Null);
    }

    [Test]
    public async Task jmongo_batch_remove_test()
    {
        var collection = JMongo<Sample>.Create(_client).GetCollection();
        var filter = Builders<Sample>.Filter.Eq(m => m.Name, "N_TEST");
        var result = await collection.DeleteManyAsync(filter);
        
    }

    [TearDown]
    public void tear_down()
    {
        _client?.Dispose();
    }
}

[JMongoCollection("demo", "sample")]
public class Sample : JMongoObjectBase
{
    [BsonElement("name")]
    public string Name { get; set; }
    
    [BsonElement("age")]
    public int Age { get; set; }
}

public static class Extensions
{
    public static DateTime ToKoreaDate(this DateTime utc)
    {
        TimeZoneInfo kstZone = TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time");
        return TimeZoneInfo.ConvertTimeFromUtc(utc, kstZone);
    }
    
    public static DateTime ToUtcDate(this DateTime kstDateTime)
    {
        return TimeZoneInfo.ConvertTimeToUtc(kstDateTime);
    }
    public static long ToUnixTimestamp(this DateTime utc)
    {
        return new DateTimeOffset(utc).ToUnixTimeSeconds();
    }

    public static long ToUnixTimestampMs(this DateTime utc)
    {
        return new DateTimeOffset(utc).ToUnixTimeMilliseconds();
    }
} 