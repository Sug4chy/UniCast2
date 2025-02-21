using CSharpFunctionalExtensions;
using UniCast.Domain.Common.ValueObjects;
using UniCast.Domain.Messages.Entities;

namespace UniCast.Domain.Telegram.Entities;

public sealed class TelegramMessage : Entity<IdOf<TelegramChat>>
{
    private readonly List<TelegramMessageReaction> _reactions;

    public int ExtId { get; }
    public IReadOnlyList<TelegramMessageReaction> Reactions => _reactions.AsReadOnly();
    public TelegramChat Chat { get; }
    public MessageFromMethodist SrcMessage { get; }

    private TelegramMessage(
        IdOf<TelegramChat> id,
        Maybe<List<TelegramMessageReaction>> reactions,
        int extId,
        TelegramChat chat,
        MessageFromMethodist srcMessage) : base(id)
    {
        _reactions = reactions.GetValueOrDefault([]);
        ExtId = extId;
        Chat = chat;
        SrcMessage = srcMessage;
    }

    public static Result<TelegramMessage> Create(
        Maybe<List<TelegramMessageReaction>> reactions,
        int extId,
        TelegramChat chat,
        MessageFromMethodist srcMessage)
        => Result.FailureIf(chat is null, "Не указан чат, к которому относится сообщение")
            .OnSuccessTry(
                () => ArgumentNullException.ThrowIfNull(srcMessage, nameof(srcMessage)),
                ex => ex.Message)
            .Map(() => new TelegramMessage(
                id: IdOf<TelegramChat>.New(),
                reactions: reactions,
                extId: extId,
                chat: chat!,
                srcMessage: srcMessage));
}