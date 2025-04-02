using CSharpFunctionalExtensions;
using UniCast.Domain.Common.ValueObjects;
using UniCast.Domain.Telegram.ValueObjects.Enums;

namespace UniCast.Domain.Telegram.Entities;

public abstract class TelegramChat : Entity<IdOf<TelegramChat>>
{
    public required string Title { get; init; }
    public required long ExtId { get; init; }
    public abstract TelegramChatType Type { get; set; }
    public ICollection<TelegramMessage> Messages { get; init; } = [];
}