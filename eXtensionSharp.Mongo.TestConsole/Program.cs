using eXtensionSharp.Mongo;
using eXtensionSharp.Mongo.TestConsole;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

// 1. 서비스 구성
services.AddJMongoDb("mongodb://localhost:27017", options =>
{
    options.ApplyConfiguration(new SampleDocumentConfiguration());
});

// 2. ServiceProvider 생성
var provider = services.BuildServiceProvider();

// 3. UseJMongoDb()로 초기화 수행 (builder + index + 등록)
using (var scope = provider.CreateScope())
{
    var appBuilder = new ApplicationBuilder(scope.ServiceProvider);
    appBuilder.UseJMongoDb();
}

// 4. JMongoFactory 사용
var factory = provider.GetRequiredService<IJMongoFactory>();
var collection = factory.Create<SampleDocument>().GetCollection();

var newItem = new SampleDocument
{
    Name = "test2",
    Age = 200,
    CreatedAt = DateTimeOffset.Now,
    
};

await collection.InsertOneAsync(newItem);
Console.WriteLine(newItem.Id);