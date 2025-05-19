using CSharpFunctionalExtensions;
using UniCast.Domain.Common.ValueObjects;
using UniCast.Domain.Students.Entities;
using UniCast.Domain.Telegram.ValueObjects.Enums;

namespace UniCast.Domain.Telegram.Entities;

public sealed class TelegramChat : Entity<IdOf<TelegramChat>>
{
    public required string Title { get; init; }
    public required long ExtId { get; init; }
    public ICollection<TelegramMessage> Messages { get; init; } = [];
    public IdOf<Student>? StudentId { get; set; }
    public Student? Student { get; set; }

    public Scenario? CurrentScenario { get; set; }
    public int? CurrentState { get; set; }

    public Dictionary<string, string> CurrentScenarioArgs { get; set; } = [];

    public static TelegramChat CreateNew(
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