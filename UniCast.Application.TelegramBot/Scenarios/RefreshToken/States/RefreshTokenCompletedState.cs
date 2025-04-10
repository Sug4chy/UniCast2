using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using UniCast.Domain.Telegram.Entities;

namespace UniCast.Application.TelegramBot.Scenarios.RefreshToken.States;

public sealed class RefreshTokenCompletedState : IRefreshTokenState
{
    private readonly RefreshTokenScenarioExecutor _scenarioExecutor;
    private readonly UpdateDispatcher _updateDispatcher;

    public RefreshTokenCompletedState(RefreshTokenScenarioExecutor scenarioExecutor, IServiceProvider serviceProvider)
    {
        _scenarioExecutor = scenarioExecutor;
        _updateDispatcher = serviceProvider.GetRequiredService<UpdateDispatcher>();
    }

    public async Task OnStateChangedAsync(PrivateTelegramChat chat, Update update, CancellationToken ct = default)
    {
        await _scenarioExecutor.ClearScenarioAsync(chat, ct);
        await _updateDispatcher.DispatchAsync(update, ct);
    }

    public Task HandleUserInputAsync(PrivateTelegramChat chat, Update update, CancellationToken ct = default)
        => Task.CompletedTask;
}