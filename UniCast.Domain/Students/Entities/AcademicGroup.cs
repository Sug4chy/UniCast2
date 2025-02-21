using CSharpFunctionalExtensions;
using UniCast.Domain.Common.ValueObjects;
using UniCast.Domain.Students.ValueObjects;

namespace UniCast.Domain.Students.Entities;

public sealed class AcademicGroup : Entity<IdOf<AcademicGroup>>
{
    private readonly List<Student> _students;

    public AcademicGroupName Name { get; }
    public int Course { get; }
    public IReadOnlyList<Student> Students => _students.AsReadOnly();

    private AcademicGroup(
        IdOf<AcademicGroup> id,
        AcademicGroupName name,
        int course,
        Maybe<List<Student>> maybeStudents) : base(id)
    {
        Name = name;
        Course = course;
        _students = maybeStudents.GetValueOrDefault([]);
    }

    public static Result<AcademicGroup> Create(
        AcademicGroupName name,
        int course,
        Maybe<List<Student>> maybeStudents)
    {
        if (course is < 1 or > 4)
        {
            return Result.Failure<AcademicGroup>("Номер курса должен быть между 1 и 4");
        }

        return Result.Success(new AcademicGroup(
            id: IdOf<AcademicGroup>.New(),
            name: name,
            course: course,
            maybeStudents: maybeStudents));
    }
}