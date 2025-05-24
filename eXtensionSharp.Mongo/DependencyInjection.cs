using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace eXtensionSharp.Mongo;

public static class DependencyInjection
{
    public static IServiceCollection AddJMongoDb(this IServiceCollection services, string mongoConnectionString, Action<JMongoDbOptions> configure = null)
    {
        if (string.IsNullOrWhiteSpace(mongoConnectionString))
            throw new ArgumentException("mongoConnectionString must not be null or empty.");

        var client = new MongoClient(mongoConnectionString);
        services.AddSingleton<IMongoClient>(client);
        services.AddSingleton<IJMongoFactory, JMongoFactory>();

        var options = new JMongoDbOptions();
        configure?.Invoke(options);
        services.AddSingleton(options);

        // 1. 등록된 타입 기반 IndexInitializer 자동 등록
        foreach (var initializerType in options.InitializerTypes)
        {
            services.AddSingleton(typeof(IJMongoConfiguration), initializerType);
        }

        // 2. Runner 등록: Lambda + 구현체 통합 실행
        services.AddSingleton<IJMongoIndexInitializerRunner, JMongoIndexInitializerRunner>(sp =>
            new JMongoIndexInitializerRunner(
                sp.GetServices<IJMongoConfiguration>(),
                sp
            ));

        return services;
    }
    
    public static void UseJMongoDbAsync(this ServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IJMongoIndexInitializerRunner>();
        runner.Run();
    }
}