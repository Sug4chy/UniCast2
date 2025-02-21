using CSharpFunctionalExtensions;
using UniCast.Domain.Common.ValueObjects;
using UniCast.Domain.Students.Entities;
using UniCast.Domain.Telegram.ValueObjects.Enums;

namespace UniCast.Domain.Telegram.Entities;

public sealed class PrivateTelegramChat : TelegramChat
{
    public override TelegramChatType Type => TelegramChatType.Private;
    public Student Student { get; }

    private PrivateTelegramChat(
        IdOf<TelegramChat> id,
        string title,
        long extId,
        Student student,
        Maybe<List<TelegramMessage>> maybeMessages) : base(id, title, extId, maybeMessages)
    {
        ArgumentNullException.ThrowIfNull(student, nameof(student));

        Student = student;
    }
}