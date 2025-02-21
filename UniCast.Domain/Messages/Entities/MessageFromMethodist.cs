using CSharpFunctionalExtensions;
using UniCast.Domain.Common.ValueObjects;
using UniCast.Domain.Students.Entities;

namespace UniCast.Domain.Messages.Entities;

public sealed class MessageFromMethodist : Entity<IdOf<MessageFromMethodist>>
{
    private readonly List<Student> _receivers;

    public string Body { get; }
    public string SenderUsername { get; }
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

    public static Result<MessageFromMethodist> Create(
        List<Student> receivers,
        string body,
        string senderUsername)
    {
        if (receivers is null)
        {
            return Result.Failure<MessageFromMethodist>("Не указаны получатели сообщения");
        }

        if (string.IsNullOrWhiteSpace(body))
        {
            return Result.Failure<MessageFromMethodist>("Не передано тело сообщения");
        }

        if (string.IsNullOrWhiteSpace(senderUsername))
        {
            return Result.Failure<MessageFromMethodist>("Не указан автор сообщения");
        }

        return Result.Success(new MessageFromMethodist(
            id: IdOf<MessageFromMethodist>.New(),
            receivers: receivers,
            body: body,
            senderUsername: senderUsername));
    }
}