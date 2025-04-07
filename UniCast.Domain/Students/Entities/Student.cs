using CSharpFunctionalExtensions;
using UniCast.Domain.Common.ValueObjects;
using UniCast.Domain.Messages.Entities;
using UniCast.Domain.Moodle;
using UniCast.Domain.Students.ValueObjects;
using UniCast.Domain.Telegram.Entities;

namespace UniCast.Domain.Students.Entities;

/// <summary>
/// Студент, который учится на ИИТ
/// </summary>
public sealed class Student : Entity<IdOf<Student>>
{
    /// <summary>
    /// ФИО студента
    /// </summary>
    public StudentFullName FullName { get; init; }

    public IdOf<AcademicGroup> GroupId { get; init; }

    /// <summary>
    /// Академическая группа, к которой принадлежит студент
    /// </summary>
    public AcademicGroup? Group { get; init; }

    /// <summary>
    /// Сообщения, адресованные студенту
    /// </summary>
    public ICollection<MessageFromMethodist> Messages { get; init; }

    public PrivateTelegramChat? TelegramChat { get; init; }

    public MoodleAccount? MoodleAccount { get; init; }

    public static Student Create(
        IdOf<Student> id,
        StudentFullName fullName,
        AcademicGroup group)
        => new()
        {
            Id = id,
            FullName = fullName,
            GroupId = group.Id,
            Group = group,
            Messages = []
        };
}