using DbUp;
using Extensions.Hosting.AsyncInitialization;

namespace UniCast.Infrastructure.Database.Initialization;

public sealed class MigrationsAsyncInitializer : IAsyncInitializer
{
    private readonly string _connectionString;

    public MigrationsAsyncInitializer(string connectionString)
    {
        _connectionString = connectionString;
    }

    public Task InitializeAsync(CancellationToken cancellationToken)
    {
        EnsureDatabase.For.PostgresqlDatabase(_connectionString);

        var engine = DeployChanges.To
            .PostgresqlDatabase(_connectionString)
            .WithScriptsEmbeddedInAssembly(GetType().Assembly)
            .Build();

        if (engine.IsUpgradeRequired())
        {
            engine.PerformUpgrade();
        }

        return Task.CompletedTask;
    }
}