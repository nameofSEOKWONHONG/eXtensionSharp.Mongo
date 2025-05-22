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
            services.AddSingleton(typeof(IJMongoIndexInitializer), initializerType);
        }

        // 2. Runner 등록: Lambda + 구현체 통합 실행
        services.AddSingleton<IJMongoIndexInitializerRunner, JMongoIndexInitializerRunner>(sp =>
            new JMongoIndexInitializerRunner(
                options.LambdaInitializers,
                sp.GetServices<IJMongoIndexInitializer>(),
                sp
            ));

        return services;
    }
    
    public static async Task UseJMongoDbAsync(this ServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IJMongoIndexInitializerRunner>();
        await runner.RunAsync();
    }
}