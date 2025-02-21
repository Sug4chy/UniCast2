using CSharpFunctionalExtensions;
using UniCast.Domain.Common.ValueObjects;
using UniCast.Domain.Students.Entities;

namespace UniCast.Domain.Messages.Entities;

/// <summary>
/// Сообщение от методиста, присланное из Moodle
/// </summary>
public sealed class MessageFromMethodist : Entity<IdOf<MessageFromMethodist>>
{
    private readonly List<Student> _receivers;

    /// <summary>
    /// Текст сообщения
    /// </summary>
    public string Body { get; }

    /// <summary>
    /// Имя отправившего сообщение пользователя в системе Moodle
    /// </summary>
    public string SenderUsername { get; }

    /// <summary>
    /// Список студентов, кому это сообщение адресовано
    /// </summary>
    public IReadOnlyList<Student> Receivers => _receivers.AsReadOnly();

    private MessageFromMethodist(
        IdOf<MessageFromMethodist> id,
        List<Student> receivers,
        string body,
        string senderUsername) : base(id)
    {
        _receivers = receivers;
        Body = body;
        SenderUsername = senderUsername;
    }
}