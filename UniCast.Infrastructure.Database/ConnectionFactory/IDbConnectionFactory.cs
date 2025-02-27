using System.Data.Common;

namespace UniCast.Infrastructure.Database.ConnectionFactory;

public interface IDbConnectionFactory
{
    Task<DbConnection> ConnectAsync(CancellationToken ct = default);
}