using eXtensionSharp.Mongo;
using eXtensionSharp.Mongo.TestConsole;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddJMongoDb("[enter mongodb connection string]", options =>
{
    options.AddInitializer<SampleDocumentConfiguration>();
});

var provider = services.BuildServiceProvider();
// 스코프를 생성하고 인덱스 초기화 실행
using (var scope = provider.CreateScope())
{
    var runner = scope.ServiceProvider.GetRequiredService<IJMongoIndexInitializerRunner>();
    runner.Run();
    // 만약 비동기 메서드라면: await runner.RunAsync();
}

var factory = provider.GetRequiredService<IJMongoFactory>();
var collection = factory.Create<SampleDocument>().GetCollection();
var newItem = new SampleDocument { Name = "test", Age = 100 };
await collection.InsertOneAsync(newItem);
Console.WriteLine(newItem.Id);