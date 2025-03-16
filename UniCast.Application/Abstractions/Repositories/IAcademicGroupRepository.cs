using UniCast.Domain.Students.Entities;
using UniCast.Domain.Students.ValueObjects;

namespace UniCast.Application.Abstractions.Repositories;

public interface IAcademicGroupRepository
{
    Task<bool> ExistsByNameAsync(AcademicGroupName name, CancellationToken ct = default);
    Task<AcademicGroup?> GetByNameAsync(AcademicGroupName name, CancellationToken ct = default);
    Task<IEnumerable<AcademicGroupName>> GetAllNamesAsync(CancellationToken ct = default);
}