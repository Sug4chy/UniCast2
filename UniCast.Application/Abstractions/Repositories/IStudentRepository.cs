using UniCast.Domain.Students.Entities;

namespace UniCast.Application.Abstractions.Repositories;

public interface IStudentRepository
{
    Task AddStudentAsync(Student student, CancellationToken ct = default);
}