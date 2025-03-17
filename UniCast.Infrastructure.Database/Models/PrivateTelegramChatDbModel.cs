using System.Text.Json;
using CSharpFunctionalExtensions;
using UniCast.Domain.Common.ValueObjects;
using UniCast.Domain.Telegram.Entities;
using UniCast.Domain.Telegram.ValueObjects.Enums;

namespace UniCast.Infrastructure.Database.Models;

public sealed record PrivateTelegramChatDbModel(
    Guid Id,
    string Title,
    long ExtId,
    short Type,
    Guid? StudentId,
    int? CurrentScenario,
    int? CurrentState,
    string? CurrentScenarioArgs
)
{
    private PrivateTelegramChatDbModel(
        Guid id,
        string title,
        long extId,
        short type,
        Guid studentId) : this(id, title, extId, type, studentId, null, null, null)
    {
    }

    public PrivateTelegramChat ToDomain()
    {
        var chat = PrivateTelegramChat.CreateNew(
            id: IdOf<TelegramChat>.From(Id),
            title: Title,
            extId: ExtId
        );
        chat.CurrentScenario = (Scenario?)CurrentScenario ?? Maybe<Scenario>.None;
        chat.CurrentState = CurrentState ?? Maybe<int>.None;
        chat.CurrentScenarioArgs = CurrentScenarioArgs is null
            ? []
            : JsonSerializer.Deserialize<Dictionary<string, string>>(CurrentScenarioArgs)!;

        return chat;
    }
}