using CSharpFunctionalExtensions;
using UniCast.Domain.Common.ValueObjects;
using UniCast.Domain.Students.Entities;
using UniCast.Domain.Telegram.ValueObjects.Enums;

namespace UniCast.Domain.Telegram.Entities;

public sealed class PrivateTelegramChat : TelegramChat
{
    private Maybe<Student> _student;

    public override TelegramChatType Type => TelegramChatType.Private;

    public Maybe<Student> Student
    {
        get => _student;
        set
        {
            if (_student == Maybe<Student>.None)
            {
                _student = value;
            }
        }
    }

    public Maybe<Scenario> CurrentScenario { get; set; }
    public Maybe<int> CurrentState { get; set; }
    public Dictionary<string, string> CurrentScenarioArgs { get; set; } = [];

    private PrivateTelegramChat(
        IdOf<TelegramChat> id,
        string title,
        long extId,
        Maybe<Student> student,
        Maybe<List<TelegramMessage>> maybeMessages) : base(id, title, extId, maybeMessages)
    {
        _student = student;
    }

    public static PrivateTelegramChat CreateNew(
        IdOf<TelegramChat> id,
        string title,
        long extId,
        Maybe<Student> student = default,
        Maybe<List<TelegramMessage>> maybeMessages = default)
        => new(id, title, extId, student, maybeMessages);
}