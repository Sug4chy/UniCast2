using UniCast.Domain.Common.ValueObjects;
using UniCast.Domain.Telegram.Entities;

namespace UniCast.Infrastructure.Database.Models;

public sealed record TelegramMessageReactionDbModel(
    Guid Id,
    string ReactorUsername,
    string Reaction,
    Guid MessageId
)
{
    public TelegramMessageReaction ToDomain(TelegramMessage message)
        => TelegramMessageReaction.Create(
            id: IdOf<TelegramMessageReaction>.From(Id),
            reactorUsername: ReactorUsername,
            reaction: Reaction,
            message: message
        ).Value;
}