using CSharpFunctionalExtensions;
using UniCast.Domain.Common.ValueObjects;
using UniCast.Domain.Students.Entities;
using UniCast.Domain.Telegram.Entities;

namespace UniCast.Domain.Messages.Entities;

/// <summary>
/// Сообщение от методиста, присланное из Moodle
/// </summary>
public sealed class MessageFromMethodist : Entity<IdOf<MessageFromMethodist>>
{
    /// <summary>
    /// Текст сообщения
    /// </summary>
    public required string Body { get; init; }

    /// <summary>
    /// Имя отправившего сообщение пользователя в системе Moodle
    /// </summary>
    public required string SenderUsername { get; init; }

    /// <summary>
    /// Список студентов, кому это сообщение адресовано
    /// </summary>
    public ICollection<Student> Students { get; set; }

    /// <summary>
    /// Сообщения в Telegram, которые были отправлены в рамках этой рассылки
    /// </summary>
    public ICollection<TelegramMessage> TelegramMessages { get; init; }

    public static MessageFromMethodist Create(
        IdOf<MessageFromMethodist> id,
        string body,
        string senderUsername)
    {
        ArgumentException.ThrowIfNullOrEmpty(body, nameof(body));
        ArgumentException.ThrowIfNullOrWhiteSpace(senderUsername, nameof(senderUsername));

        return new MessageFromMethodist
        {
            Id = id,
            Body = body,
            SenderUsername = senderUsername,
            Students = [],
            TelegramMessages = []
        };
    }
}