namespace UniCast.Infrastructure.Database.Models;

public sealed record StudentDbModel(
    Guid Id,
    string FullName,
    Guid GroupId
);