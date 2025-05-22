using Microsoft.Extensions.DependencyInjection;

namespace eXtensionSharp.Mongo.xTest;

public class JMongoIntegrationTestContext
{
    public ServiceProvider Provider { get; }

    public JMongoIntegrationTestContext()
    {
        var services = new ServiceCollection();

        // 실제 Mongo 연결 문자열 사용 (테스트 DB 전용)
        services.AddJMongoDb("[enter mongodb connection string]");

        Provider = services.BuildServiceProvider();
    }
}