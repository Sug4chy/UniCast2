using CSharpFunctionalExtensions;
using UniCast.Domain.Common.ValueObjects;

namespace UniCast.Domain.Telegram.Entities;

public sealed class TelegramMessageReaction : Entity<IdOf<TelegramMessageReaction>>
{
    public required string ReactorUsername { get; init; }
    public required string Reaction { get; init; }

    public IdOf<TelegramMessage> MessageId { get; init; }
    public TelegramMessage? Message { get; init; }

    public static TelegramMessageReaction Create(
        IdOf<TelegramMessageReaction> id,
        string reactorUsername,
        string reaction,
        TelegramMessage message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reactorUsername, nameof(reactorUsername));
        ArgumentException.ThrowIfNullOrWhiteSpace(reaction, nameof(reaction));

        return new TelegramMessageReaction
        {
            Id = id,
            ReactorUsername = reactorUsername,
            Reaction = reaction,
            MessageId = message.Id,
            Message = message
        };
    }
}