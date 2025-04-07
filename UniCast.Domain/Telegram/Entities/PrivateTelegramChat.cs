using UniCast.Domain.Common.ValueObjects;
using UniCast.Domain.Students.Entities;
using UniCast.Domain.Telegram.ValueObjects.Enums;

namespace UniCast.Domain.Telegram.Entities;

public sealed class PrivateTelegramChat : TelegramChat
{
    public override TelegramChatType Type { get; set; } = TelegramChatType.Private;

    public IdOf<Student>? StudentId { get; set; }
    public Student? Student { get; set; }

    public Scenario? CurrentScenario { get; set; }
    public int? CurrentState { get; set; }

    public Dictionary<string, string> CurrentScenarioArgs { get; set; } = [];

    public static PrivateTelegramChat CreateNew(
        IdOf<TelegramChat> id,
        string title,
        long extId,
        Student? student = null)
        => new()
        {
            Id = id,
            Title = title,
            ExtId = extId,
            StudentId = student?.Id,
            Student = student,
            Messages = []
        };
}