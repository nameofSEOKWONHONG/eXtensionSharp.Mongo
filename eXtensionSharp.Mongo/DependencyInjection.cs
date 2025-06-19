using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace eXtensionSharp.Mongo;

public static class DependencyInjection
{
    public static IServiceCollection AddJMongoDb(
        this IServiceCollection services,
        string connectionString,
        Action<JMongoConfigurationRegistry> configure)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("MongoDB connection string is required.");

        services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));
        services.AddSingleton<JMongoCollectionFactory>();
        services.AddSingleton<IJMongoFactory>(sp => sp.GetRequiredService<JMongoCollectionFactory>());
        services.AddSingleton<IJMongoFactoryBuilder>(sp => sp.GetRequiredService<JMongoCollectionFactory>());

        var options = new JMongoConfigurationRegistry();
        configure?.Invoke(options);
        services.AddSingleton(options); // 실행 시점에서만 사용됨

        return services;
    }
    
    public static void UseJMongoDb(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var options = scope.ServiceProvider.GetRequiredService<JMongoConfigurationRegistry>();
        var client  = scope.ServiceProvider.GetRequiredService<IMongoClient>();
        var builder = scope.ServiceProvider.GetRequiredService<IJMongoFactoryBuilder>();

        foreach (var exec in options.Executors)
        {
            exec(client, builder); // 리플렉션, typeof, cast 전혀 없음
        }
    }
}