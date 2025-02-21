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
}