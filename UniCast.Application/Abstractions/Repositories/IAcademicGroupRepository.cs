using UniCast.Domain.Common.ValueObjects;
using UniCast.Domain.Students.Entities;

namespace UniCast.Application.Abstractions.Repositories;

public interface IAcademicGroupRepository
{
    Task<AcademicGroup?> GetByNameAsync(string name, CancellationToken ct = default);
}