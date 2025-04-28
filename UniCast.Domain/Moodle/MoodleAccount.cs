using CSharpFunctionalExtensions;
using UniCast.Domain.Common.ValueObjects;
using UniCast.Domain.Students.Entities;

namespace UniCast.Domain.Moodle;

public sealed class MoodleAccount(IdOf<MoodleAccount> id) : Entity<IdOf<MoodleAccount>>(id)
{
    public long ExtId { get; init; }
    public string Username { get; init; }
    public string CurrentToken { get; set; }

    public IdOf<Student>? StudentId { get; init; }
    public Student? Student { get; init; }
}