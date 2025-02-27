using CSharpFunctionalExtensions;
using UniCast.Domain.Common.ValueObjects;
using UniCast.Domain.Students.Entities;
using UniCast.Domain.Students.ValueObjects;

namespace UniCast.Infrastructure.Database.Models;

public sealed record AcademicGroupDbModel(
    Guid Id,
    string Name
)
{
    public AcademicGroup ToDomain()
    {
        var name = AcademicGroupName.From(Name).Value;

        return AcademicGroup.Create(
            id: IdOf<AcademicGroup>.From(Id),
            name: name,
            maybeStudents: Maybe<List<Student>>.None
        );
    }
}