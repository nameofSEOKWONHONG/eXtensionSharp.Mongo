using eXtensionSharp.Mongo;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Moq;

namespace eXtensionSharp.Mongo.Test;

// 테스트용 컬렉션 도메인 클래스
[JMongoCollection("TestDb", "TestCollection")]
public class TestDocument : JMongoObjectBase
{
    public string Name { get; set; }
}

public class DummyMongoConfig : IJMongoConfiguration
{
    public bool Called { get; private set; } = false;

    public void Configure(IJMongoFactory factory)
    {
        // 정상 호출 시 플래그 변경
        Called = true;
    }
}

public class JMongoTests
{
    [Fact]
    public void JMongoCollectionAttribute_Should_Set_Properties()
    {
        var attr = new JMongoCollectionAttribute("db", "col");
        Assert.Equal("db", attr.DatabaseName);
        Assert.Equal("col", attr.CollectionName);
    }

    [Fact]
    public void JMongoCollectionAttribute_Should_Throw_On_Empty_Arg()
    {
        Assert.Throws<ArgumentException>(() => new JMongoCollectionAttribute("", "col"));
        Assert.Throws<ArgumentException>(() => new JMongoCollectionAttribute("db", ""));
    }

    [Fact]
    public void JMongo_Should_Throw_If_Args_Null()
    {
        var client = new Mock<IMongoClient>().Object;
        Assert.Throws<ArgumentNullException>(() => new JMongo<TestDocument>(null, "col", client));
        Assert.Throws<ArgumentNullException>(() => new JMongo<TestDocument>("db", null, client));
        Assert.Throws<ArgumentNullException>(() => new JMongo<TestDocument>("db", "col", null));
    }

    [Fact]
    public void JMongoFactory_Should_Throw_If_No_Attribute()
    {
        var client = new Mock<IMongoClient>().Object;
        var factory = new JMongoFactory(client);
        Assert.Throws<InvalidOperationException>(() => factory.Create<object>());
    }

    [Fact]
    public void JMongoFactory_Should_Create_JMongo_Instance()
    {
        var client = new Mock<IMongoClient>().Object;
        var factory = new JMongoFactory(client);
        var jmongo = factory.Create<TestDocument>();
        Assert.NotNull(jmongo);
    }

    [Fact]
    public void JMongoDbOptions_Should_Add_Initializer()
    {
        var options = new JMongoDbOptions();
        options.AddInitializer<DummyMongoConfig>();
        Assert.Contains(typeof(DummyMongoConfig), options.GetType().GetProperty("InitializerTypes", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(options) as List<Type>);
    }

    [Fact]
    public void DependencyInjection_Should_Register_Services()
    {
        var services = new ServiceCollection();
        var optionsChecked = false;

        services.AddJMongoDb(
            "mongodb://localhost:27017",
            opt =>
            {
                opt.AddInitializer<DummyMongoConfig>();
                optionsChecked = true;
            });

        var provider = services.BuildServiceProvider();
        Assert.True(optionsChecked);
        Assert.NotNull(provider.GetService<IMongoClient>());
        Assert.NotNull(provider.GetService<IJMongoFactory>());
        Assert.NotNull(provider.GetService<IJMongoIndexInitializerRunner>());
    }

    [Fact]
    public void JMongoIndexInitializerRunner_Should_Run_Configure()
    {
        var configs = new List<DummyMongoConfig> { new DummyMongoConfig() };
        var services = new ServiceCollection();
        services.AddSingleton<IJMongoConfiguration>(configs[0]);
        services.AddSingleton<IJMongoFactory>(new Mock<IJMongoFactory>().Object);
        var provider = services.BuildServiceProvider();

        var runner = new JMongoIndexInitializerRunner(configs, provider);
        runner.Run();

        Assert.True(configs[0].Called);
    }

    [Fact]
    public void ObjectIdConverter_Should_Serialize_And_Deserialize()
    {
        var converter = new ObjectIdConverter();
        var id = MongoDB.Bson.ObjectId.GenerateNewId();
        var options = new System.Text.Json.JsonSerializerOptions();
        options.Converters.Add(converter);

        var json = System.Text.Json.JsonSerializer.Serialize(id, options);
        var parsed = System.Text.Json.JsonSerializer.Deserialize<MongoDB.Bson.ObjectId>(json, options);

        Assert.Equal(id, parsed);
    }
}
