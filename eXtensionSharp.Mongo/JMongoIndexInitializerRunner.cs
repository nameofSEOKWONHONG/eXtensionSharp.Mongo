using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace eXtensionSharp.Mongo;

public sealed class JMongoIndexInitializerRunner : IJMongoIndexInitializerRunner
{
    private readonly IEnumerable<IJMongoConfiguration> _implementations;
    private readonly IServiceProvider _provider;

    public JMongoIndexInitializerRunner(
        IEnumerable<IJMongoConfiguration> implementations,
        IServiceProvider provider)
    {
        _implementations = implementations;
        _provider = provider;
    }

    public void Run()
    {
        var factory = _provider.GetRequiredService<IJMongoFactory>();
        foreach (var impl in _implementations)
            impl.Configure(factory);
    }
}