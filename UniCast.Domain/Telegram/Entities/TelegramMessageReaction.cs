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
        IdOf<TelegramMessageReaction> id,
        string reactorUsername,
        string reaction,
        TelegramMessage message)
        => Result.FailureIf(string.IsNullOrWhiteSpace(reactorUsername), "Reactor username can't be empty.")
            .Bind(() => Result.FailureIf(string.IsNullOrWhiteSpace(reaction), "Reaction can't be empty."))
            .Map(() => new TelegramMessageReaction(id, reactorUsername, reaction, message));
}