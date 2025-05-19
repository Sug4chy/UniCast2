using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using UniCast.Infrastructure.Persistence.Context;
using UniCast.Infrastructure.Persistence.Context.Options;

using var cts = new CancellationTokenSource();
var ct = cts.Token;

string envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? Environments.Development;

var definiteEnvMigrationNameRegex = new Regex(
    $"\\d+_({Environments.Development}|{Environments.Staging}|{Environments.Production})__.+.cs");

var configuration =  new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .AddJsonFile($"appsettings.{envName}.json")
    .Build();

string? connectionString = configuration.GetConnectionString("DefaultConnection");

await using var dbContext = new PostgresqlDataContext(
    DbContextOptionsFactory.Build<PostgresqlDataContext>(connectionString));

var pendingMigrationsNames =  await dbContext.Database.GetPendingMigrationsAsync(ct);
foreach (string migrationName in pendingMigrationsNames)
{
    if (definiteEnvMigrationNameRegex.IsMatch(migrationName))
    {
        if (migrationName.Contains($"_{envName}__"))
        {
            await dbContext.Database.MigrateAsync(migrationName, ct);
        }

        continue;
    }

    await dbContext.Database.MigrateAsync(migrationName, ct);
    Console.WriteLine($"Applied  migration: {migrationName}");
}

Console.WriteLine("Finished");