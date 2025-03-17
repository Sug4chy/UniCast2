using Dapper;
using UniCast.Application.Abstractions.Repositories;
using UniCast.Domain.Students.Entities;
using UniCast.Infrastructure.Database.Attributes;
using UniCast.Infrastructure.Database.ConnectionFactory;
using UniCast.Infrastructure.Database.Extensions;
using UniCast.Infrastructure.Database.Models;

namespace UniCast.Infrastructure.Database.Repositories;

[Repository]
public sealed class StudentRepository : IStudentRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public StudentRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task AddStudentAsync(Student student, CancellationToken ct = default)
    {
        const string command = $"""
                                INSERT INTO student(id, full_name, group_id)
                                VALUES (
                                        @{nameof(StudentDbModel.Id)},
                                        @{nameof(StudentDbModel.FullName)},
                                        @{nameof(StudentDbModel.GroupId)}
                                )
                                """;
        await using var connection = await _dbConnectionFactory.ConnectAsync(ct);

        await connection.ExecuteAsync(command, student.ToDbModel());
    }
}