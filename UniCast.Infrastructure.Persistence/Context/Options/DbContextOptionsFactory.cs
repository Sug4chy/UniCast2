using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;

namespace UniCast.Infrastructure.Persistence.Context.Options;

public static class DbContextOptionsFactory
{
    private static readonly ILoggerFactory SqlLoggerFactory
        = LoggerFactory.Create(builder => builder.AddSerilog());
    
    public static DbContextOptions<TContext> Build<TContext>(string? conString)
        where TContext : DbContext
        => new DbContextOptionsBuilder<TContext>()
            .UseNpgsql(conString)
            .UseLoggerFactory(SqlLoggerFactory)
            .Options;
}