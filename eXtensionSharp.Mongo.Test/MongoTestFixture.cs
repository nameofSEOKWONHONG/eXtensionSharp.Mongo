using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Mongo2Go;

namespace eXtensionSharp.Mongo.Test;

public class MongoTestFixture : IDisposable
{
    public IServiceProvider ServiceProvider { get; }

    private readonly MongoDbRunner _runner;

    public MongoTestFixture()
    {
        _runner = MongoDbRunner.Start();

        var services = new ServiceCollection();

        services.AddJMongoDb(_runner.ConnectionString, options =>
        {
            options.ApplyConfiguration(new SampleDocumentConfiguration());
        });

        ServiceProvider = services.BuildServiceProvider();

        // 구성된 컬렉션 및 인덱스 실행
        var appBuilder = new ApplicationBuilder(ServiceProvider);
        appBuilder.UseJMongoDb(); // 인덱스 등록 등 적용
    }

    public void Dispose() => _runner.Dispose();
}