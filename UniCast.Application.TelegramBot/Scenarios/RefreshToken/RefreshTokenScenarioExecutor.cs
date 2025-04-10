using Telegram.Bot.Types;
using UniCast.Application.Abstractions.Persistence;
using UniCast.Application.TelegramBot.Scenarios.RefreshToken.States;
using UniCast.Domain.Telegram.Entities;
using UniCast.Domain.Telegram.ValueObjects.Enums;

namespace UniCast.Application.TelegramBot.Scenarios.RefreshToken;

public sealed class RefreshTokenScenarioExecutor : IScenarioExecutor<IRefreshTokenState>
{
    private readonly IDataContext _dataContext;
    private readonly IServiceProvider _serviceProvider;

    public RefreshTokenScenarioExecutor(IDataContext dataContext, IServiceProvider serviceProvider)
    {
        _dataContext = dataContext;
        _serviceProvider = serviceProvider;
    }

    public Scenario Scenario => Scenario.RefreshToken;

    public async Task StartScenarioAsync(PrivateTelegramChat chat, Update update, CancellationToken ct = default)
    {
        chat.CurrentScenario = Scenario.RefreshToken;
        chat.CurrentState = (int)RefreshTokenScenarioState.Started;

        await _dataContext.SaveChangesAsync(ct);
        await GetState((int)RefreshTokenScenarioState.Started)
            .OnStateChangedAsync(chat, update, ct);
    }

    public IRefreshTokenState GetState(int state)
        => state switch
        {
            (int)RefreshTokenScenarioState.Started => new RefreshTokenStartedState(this, _serviceProvider),
            (int)RefreshTokenScenarioState.Completed => new RefreshTokenCompletedState(this, _serviceProvider),
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
        };

    public async Task ClearScenarioAsync(PrivateTelegramChat chat, CancellationToken ct = default)
    {
        chat.CurrentScenario = null;
        chat.CurrentState = null;
        chat.CurrentScenarioArgs = [];

        await _dataContext.SaveChangesAsync(ct);
    }

    public async Task ChangeStateAsync(PrivateTelegramChat chat, IRefreshTokenState newState, Update update,
        CancellationToken ct = default)
    {
        chat.CurrentState = GetState(newState);
        await _dataContext.SaveChangesAsync(ct);
        await newState.OnStateChangedAsync(chat, update, ct);
    }

    public int GetState(IRefreshTokenState state)
        => (int)(state switch
        {
            RefreshTokenStartedState => RefreshTokenScenarioState.Started,
            RefreshTokenCompletedState => RefreshTokenScenarioState.Completed,
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
        });

    IState IScenarioExecutor.GetState(int state) 
        => GetState(state);

    public ValueTask<bool> CanStartScenarioAsync(Update update, CancellationToken ct = default)
        => throw new NotSupportedException();
}