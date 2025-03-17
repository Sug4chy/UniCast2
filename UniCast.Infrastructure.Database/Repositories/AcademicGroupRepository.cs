using Dapper;
using UniCast.Application.Abstractions.Repositories;
using UniCast.Domain.Students.Entities;
using UniCast.Domain.Students.ValueObjects;
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

    public async Task<bool> ExistsByNameAsync(AcademicGroupName name, CancellationToken ct = default)
    {
        const string query = $"""
                              SELECT EXISTS(
                                SELECT id 
                                FROM academic_group
                                WHERE name=@{nameof(name)}
                              );
                              """;
        await using var connection = await _dbConnectionFactory.ConnectAsync(ct);

        return await connection.QuerySingleAsync<bool>(query, new { name = name.ToString() });
    }

    public async Task<AcademicGroup?> GetByNameAsync(AcademicGroupName name, CancellationToken ct = default)
    {
        const string query = $"""
                              SELECT id AS {nameof(AcademicGroupDbModel.Id)},
                                     name AS {nameof(AcademicGroupDbModel.Name)}
                              FROM academic_group
                              WHERE name = @{nameof(name)};
                              """;
        await using var connection = await _dbConnectionFactory.ConnectAsync(ct);
        var groupDbModel = await connection.QueryFirstOrDefaultAsync<AcademicGroupDbModel>(
            query, new { name = name.ToString() });

        return groupDbModel?.ToDomain();
    }

    public async Task<IEnumerable<AcademicGroupName>> GetAllNamesAsync(CancellationToken ct = default)
    {
        const string query = """
                              SELECT name
                              FROM academic_group;
                              """;
        await using var connection = await _dbConnectionFactory.ConnectAsync(ct);

        var names = await connection.QueryAsync<string>(query);

        return names.Select(s => AcademicGroupName.From(s).Value);
    }
}