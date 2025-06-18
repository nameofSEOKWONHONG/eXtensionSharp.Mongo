using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace eXtensionSharp.Mongo;

public static class DependencyInjection
{
    public static IServiceCollection AddJMongoDb(
        this IServiceCollection services,
        string connectionString,
        Action<JMongoDbOptions> configure)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("MongoDB connection string is required.");

        services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));
        services.AddSingleton<IJMongoFactory, JMongoFactory>();
        services.AddSingleton<IJMongoFactoryBuilder>(sp => (IJMongoFactoryBuilder)sp.GetRequiredService<IJMongoFactory>());


        var options = new JMongoDbOptions();
        configure?.Invoke(options);
        services.AddSingleton(options); // 실행 시점에서만 사용됨

        return services;
    }
    
    public static void UseJMongoDb(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var options = scope.ServiceProvider.GetRequiredService<JMongoDbOptions>();
        var client  = scope.ServiceProvider.GetRequiredService<IMongoClient>();
        var factory = scope.ServiceProvider.GetRequiredService<IJMongoFactoryBuilder>();

        foreach (var exec in options.Executors)
        {
            exec(client, factory); // 리플렉션, typeof, cast 전혀 없음
        }
    }
}