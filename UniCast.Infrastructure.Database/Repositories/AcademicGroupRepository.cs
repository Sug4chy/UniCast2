using Dapper;
using UniCast.Application.Abstractions.Repositories;
using UniCast.Domain.Students.Entities;
using UniCast.Infrastructure.Database.Attributes;
using UniCast.Infrastructure.Database.ConnectionFactory;
using UniCast.Infrastructure.Database.Models;

namespace UniCast.Infrastructure.Database.Repositories;

[Repository]
public sealed class AcademicGroupRepository : IAcademicGroupRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public AcademicGroupRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<AcademicGroup?> GetByNameAsync(string name, CancellationToken ct = default)
    {
        const string query = $"""
                              SELECT id AS {nameof(AcademicGroupDbModel.Id)},
                                     name AS {nameof(AcademicGroupDbModel.Name)}
                              FROM academic_group
                              WHERE name = @{nameof(name)};
                              """;
        await using var connection = await _dbConnectionFactory.ConnectAsync(ct);
        var groupDbModel = await connection.QueryFirstOrDefaultAsync<AcademicGroupDbModel>(query, new { name });

        return groupDbModel?.ToDomain();
    }
}