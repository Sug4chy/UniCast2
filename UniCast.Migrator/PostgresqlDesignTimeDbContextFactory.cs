using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using UniCast.Infrastructure.Persistence.Context;
using UniCast.Infrastructure.Persistence.Context.Options;

namespace UniCast.Migrator;

public sealed class PostgresqlDesignTimeDbContextFactory : IDesignTimeDbContextFactory<PostgresqlDataContext>
{
    public PostgresqlDataContext CreateDbContext(string[] args)
    {
        string envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? Environments.Development;

        var configuration =  new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .AddJsonFile($"appsettings.{envName}.json")
            .Build();

        string? connectionString = configuration.GetConnectionString("DefaultConnection");

        var dbContext = new PostgresqlDataContext(
            DbContextOptionsFactory.Build<PostgresqlDataContext>(connectionString));

        return dbContext;
    }
}