using Telegram.Bot.Types;
using UniCast.Domain.Telegram.Entities;
using UniCast.Domain.Telegram.ValueObjects.Enums;

namespace UniCast.Application.TelegramBot.Scenarios;

public interface IScenarioExecutor<TState> : IScenarioExecutor
    where TState : IState
{
    Task ChangeStateAsync(
        TelegramChat chat,
        TState newState,
        Update update,
        CancellationToken ct = default);

    int GetState(TState state);
    new TState GetState(int state);
    Task ClearScenarioAsync(TelegramChat chat, CancellationToken ct = default);
}

public interface IScenarioExecutor
{
    Scenario Scenario { get; }
    Task StartScenarioAsync(TelegramChat chat, Update update, CancellationToken ct = default);
    IState GetState(int state);
    ValueTask<bool> CanStartScenarioAsync(Update update, CancellationToken ct = default);
}