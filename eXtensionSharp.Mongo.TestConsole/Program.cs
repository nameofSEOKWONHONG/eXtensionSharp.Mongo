using eXtensionSharp.Mongo;
using eXtensionSharp.Mongo.TestConsole;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddJMongoDb("[enter mongodb connection string]", options =>
{
    options.AddInitializer<SampleDocumentConfiguration>();
});

var provider = services.BuildServiceProvider();

provider.UseJMongoDbAsync();

var factory = provider.GetRequiredService<IJMongoFactory>();
var collection = factory.Create<SampleDocument>().GetCollection();
var newItem = new SampleDocument { Name = "test", Age = 100 };
await collection.InsertOneAsync(newItem);
Console.WriteLine(newItem.Id);