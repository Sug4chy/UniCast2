using CSharpFunctionalExtensions;
using UniCast.Domain.Common.ValueObjects;
using UniCast.Domain.Messages.Entities;
using UniCast.Domain.Telegram.Entities;

namespace UniCast.Infrastructure.Database.Models;

public sealed record TelegramMessageDbModel(
    Guid Id,
    int ExtId,
    Guid ChatId,
    Guid SrcMessageId
)
{
    public TelegramMessage ToDomain(List<TelegramMessageReaction> reactions)
        => TelegramMessage.Create(
            id: IdOf<TelegramChat>.From(Id),
            reactions: reactions,
            extId: ExtId,
            chat: Maybe<TelegramChat>.None,
            srcMessage: Maybe<MessageFromMethodist>.None
        );

    public TelegramMessage ToDomain()
        => TelegramMessage.Create(
            id: IdOf<TelegramChat>.From(Id),
            reactions: Maybe<List<TelegramMessageReaction>>.None,
            extId: ExtId,
            chat: Maybe<TelegramChat>.None,
            srcMessage: Maybe<MessageFromMethodist>.None
        );
}