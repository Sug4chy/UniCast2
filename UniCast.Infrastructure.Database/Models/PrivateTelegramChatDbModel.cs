namespace UniCast.Infrastructure.Database.Models;

public sealed record PrivateTelegramChatDbModel(
    Guid Id,
    string Title,
    long ExtId,
    byte Type,
    Guid? StudentId
);