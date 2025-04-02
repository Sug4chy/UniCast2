using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Serilog;

namespace UniCast.Infrastructure.Persistence.Context.Options;

public static class DbContextOptionsFactory
{
    private static readonly ILoggerFactory SqlLoggerFactory
        = LoggerFactory.Create(builder => builder.AddSerilog());

    public static DbContextOptions<TContext> Build<TContext>(string? conString)
        where TContext : DbContext
        => new DbContextOptionsBuilder<TContext>()
            .UseNpgsql(
                new NpgsqlDataSourceBuilder(conString)
                    .EnableDynamicJson([typeof(Dictionary<string, string>)])
                    .Build()
            )
            .UseLoggerFactory(SqlLoggerFactory)
            .Options;
}