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
}