using CSharpFunctionalExtensions;
using UniCast.Domain.Common.ValueObjects;
using UniCast.Domain.Students.ValueObjects;
using UniCast.Domain.Telegram.Entities;

namespace UniCast.Domain.Students.Entities;

/// <summary>
/// Академическая группа, в которой учатся студенты
/// </summary>
public sealed class AcademicGroup : Entity<IdOf<AcademicGroup>>
{
    /// <summary>
    /// Официальное название группы. Состоит из направления обучения, номера курса и номера группы
    /// </summary>
    public AcademicGroupName Name { get; init; }

    /// <summary>
    /// Список студентов, которые учатся в этой группе
    /// </summary>
    public ICollection<Student> Students { get; init; }

    /// <summary>
    /// Telegram канал, в который выкладываются объявления этой группы
    /// </summary>
    public TelegramChannel? TelegramChannel { get; init; }

    public static AcademicGroup Create(
        IdOf<AcademicGroup> id,
        AcademicGroupName name)
        => new()
        {
            Id = id,
            Name = name,
            Students = []
        };
}