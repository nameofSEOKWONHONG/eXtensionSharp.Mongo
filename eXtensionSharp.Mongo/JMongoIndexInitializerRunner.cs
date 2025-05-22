using MongoDB.Driver;

namespace eXtensionSharp.Mongo;

internal sealed class JMongoIndexInitializerRunner : IJMongoIndexInitializerRunner
{
    private readonly IEnumerable<Func<IServiceProvider, Task>> _lambdas;
    private readonly IEnumerable<IJMongoIndexInitializer> _implementations;
    private readonly IServiceProvider _provider;

    public JMongoIndexInitializerRunner(
        IEnumerable<Func<IServiceProvider, Task>> lambdas,
        IEnumerable<IJMongoIndexInitializer> implementations,
        IServiceProvider provider)
    {
        _lambdas = lambdas;
        _implementations = implementations;
        _provider = provider;
    }

    public async Task RunAsync()
    {
        foreach (var lambda in _lambdas)
            await lambda(_provider);

        foreach (var impl in _implementations)
            await impl.InitializeIndexesAsync(_provider);
    }
}