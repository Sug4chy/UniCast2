using CSharpFunctionalExtensions;
using UniCast.Domain.Common.ValueObjects;

namespace UniCast.Domain.Telegram.Entities;

public sealed class TelegramMessageReaction : Entity<IdOf<TelegramMessageReaction>>
{
    public string ReactorUsername { get; }
    public string Reaction { get; }
    public TelegramMessage Message { get; }

    private TelegramMessageReaction(
        IdOf<TelegramMessageReaction> id,
        string reactorUsername,
        string reaction,
        TelegramMessage message) : base(id)
    {
        ReactorUsername = reactorUsername;
        Reaction = reaction;
        Message = message;
    }

    public static Result<TelegramMessageReaction> Create(
        string reactorUsername,
        string reaction,
        TelegramMessage message)
    {
        if (message is null)
        {
            return Result.Failure<TelegramMessageReaction>("Не указано сообщение, на которое поставлена реакция");
        }

        if (string.IsNullOrWhiteSpace(reactorUsername))
        {
            return Result.Failure<TelegramMessageReaction>("Имя отреагировавшего пользователя отсутствует");
        }

        if (string.IsNullOrWhiteSpace(reaction))
        {
            return Result.Failure<TelegramMessageReaction>("Реакция отсутствует");
        }

        return Result.Success(new TelegramMessageReaction(
            id: IdOf<TelegramMessageReaction>.New(),
            reactorUsername: reactorUsername,
            reaction: reaction,
            message: message));
    }
}