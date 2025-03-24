using Extensions.Hosting.AsyncInitialization;
using Microsoft.EntityFrameworkCore;
using UniCast.Infrastructure.Persistence.Context;

namespace UniCast.Infrastructure.Persistence.Initialization;

public sealed class MigrationsAsyncInitializer : IAsyncInitializer
{
    private readonly PostgresqlDataContext _dataContext;

    public MigrationsAsyncInitializer(PostgresqlDataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task InitializeAsync(CancellationToken ct)
    {
        if ((await _dataContext.Database.GetPendingMigrationsAsync(ct)).Any())
        {
            await _dataContext.Database.MigrateAsync(ct);
        }
    }
}