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

    /// <summary>
    /// Сообщения, адресованные студенту
    /// </summary>
    public ICollection<MessageFromMethodist> Messages { get; init; } = [];

    public TelegramChat? TelegramChat { get; init; }

    public MoodleAccount? MoodleAccount { get; init; }

    public static Student Create(
        IdOf<Student> id,
        StudentFullName fullName)
        => new()
        {
            Id = id,
            FullName = fullName,
            Messages = []
        };
}