using System.Data.Common;
using Npgsql;

namespace UniCast.Infrastructure.Database.ConnectionFactory;

public sealed class NpgsqlDbConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public NpgsqlDbConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<DbConnection> ConnectAsync(CancellationToken ct = default)
    {
        var con = new NpgsqlConnection(_connectionString);
        await con.OpenAsync(ct);

        return con;
    }
}