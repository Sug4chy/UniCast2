using CSharpFunctionalExtensions;
using UniCast.Domain.Common.ValueObjects;
using UniCast.Domain.Telegram.ValueObjects.Enums;

namespace UniCast.Domain.Telegram.Entities;

public abstract class TelegramChat : Entity<IdOf<TelegramChat>>
{
    protected readonly List<TelegramMessage> _messages;

    public string Title { get; }
    public long ExtId { get; }
    public abstract TelegramChatType Type { get; }
    public IReadOnlyList<TelegramMessage> Messages => _messages.AsReadOnly();

    protected TelegramChat(
        IdOf<TelegramChat> id, 
        string title, 
        long extId,
        Maybe<List<TelegramMessage>> maybeMessages) : base(id)
    {
        ArgumentNullException.ThrowIfNull(title);

        Title = title;
        ExtId = extId;
        _messages = maybeMessages.GetValueOrDefault([]);
    }
}