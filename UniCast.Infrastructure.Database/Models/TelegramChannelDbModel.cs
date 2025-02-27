namespace UniCast.Infrastructure.Database.Models;

public sealed record TelegramChannelDbModel(
    Guid Id,
    string Title,
    long ExtId,
    byte Type,
    Guid GroupId
);