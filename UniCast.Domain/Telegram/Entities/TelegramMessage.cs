using CSharpFunctionalExtensions;
using UniCast.Domain.Common.ValueObjects;
using UniCast.Domain.Messages.Entities;

namespace UniCast.Domain.Telegram.Entities;

public sealed class TelegramMessage : Entity<IdOf<TelegramChat>>
{
    private readonly List<TelegramMessageReaction> _reactions;

    public int ExtId { get; }
    public IReadOnlyList<TelegramMessageReaction> Reactions => _reactions.AsReadOnly();
    public Maybe<TelegramChat> Chat { get; }
    public Maybe<MessageFromMethodist> SrcMessage { get; }

    private TelegramMessage(
        IdOf<TelegramChat> id,
        Maybe<List<TelegramMessageReaction>> reactions,
        int extId,
        Maybe<TelegramChat> chat,
        Maybe<MessageFromMethodist> srcMessage) : base(id)
    {
        _reactions = reactions.GetValueOrDefault([]);
        ExtId = extId;
        Chat = chat;
        SrcMessage = srcMessage;
    }

    public static TelegramMessage Create(
        IdOf<TelegramChat> id,
        Maybe<List<TelegramMessageReaction>> reactions,
        int extId,
        Maybe<TelegramChat> chat,
        Maybe<MessageFromMethodist> srcMessage)
        => new(
            id: id,
            reactions: reactions,
            extId: extId,
            chat: chat,
            srcMessage: srcMessage
        );

    public void AddReaction(TelegramMessageReaction reaction)
    {
        if (_reactions.Find(r => r == reaction) is null)
        {
            _reactions.Add(reaction);
        }
    }

    public void AddReactions(List<TelegramMessageReaction> reactions)
        => reactions.ForEach(AddReaction);
}