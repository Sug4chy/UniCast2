using CSharpFunctionalExtensions;
using UniCast.Domain.Common.ValueObjects;
using UniCast.Domain.Messages.Entities;

namespace UniCast.Domain.Telegram.Entities;

public sealed class TelegramMessage : Entity<IdOf<TelegramMessage>>
{
    public int ExtId { get; init; }
    public ICollection<TelegramMessageReaction> Reactions { get; init; }
    public IdOf<TelegramChat> ChatId { get; init; }
    public TelegramChat? Chat { get; init; }
    public IdOf<MessageFromMethodist> SrcMessageId { get; set; }
    public MessageFromMethodist? SrcMessage { get; set; }

    public static TelegramMessage Create(
        IdOf<TelegramMessage> id,
        int extId,
        TelegramChat chat,
        MessageFromMethodist srcMessage)
        => new()
        {
            Id = id,
            ExtId = extId,
            Reactions = [],
            ChatId = chat.Id,
            Chat = chat
        };
}