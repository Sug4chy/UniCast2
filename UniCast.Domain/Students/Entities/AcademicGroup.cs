using CSharpFunctionalExtensions;
using UniCast.Domain.Common.ValueObjects;
using UniCast.Domain.Students.ValueObjects;

namespace UniCast.Domain.Students.Entities;

/// <summary>
/// Академическая группа, в которой учатся студенты
/// </summary>
public sealed class AcademicGroup : Entity<IdOf<AcademicGroup>>
{
    private readonly List<Student> _students;

    /// <summary>
    /// Официальное название группы. Состоит из направления обучения, номера курса и номера группы
    /// </summary>
    public AcademicGroupName Name { get; }

    /// <summary>
    /// Номер курса, на котором обучается эта группа
    /// </summary>
    public int Course { get; }

    /// <summary>
    /// Список студентов, которые учатся в этой группе
    /// </summary>
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
}