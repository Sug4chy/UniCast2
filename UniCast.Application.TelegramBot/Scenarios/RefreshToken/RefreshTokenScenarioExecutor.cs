using System.Text.Json;
using Telegram.Bot.Types;
using UniCast.Application.Abstractions.Persistence;
using UniCast.Application.TelegramBot.Scenarios.RefreshToken.States;
using UniCast.Domain.Telegram.Entities;
using UniCast.Domain.Telegram.ValueObjects.Enums;
using ScenarioEnum = UniCast.Domain.Telegram.ValueObjects.Enums.Scenario;

namespace UniCast.Application.TelegramBot.Scenarios.RefreshToken;

public sealed class RefreshTokenScenarioExecutor : IScenarioExecutor<IRefreshTokenState>
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.General);

    private readonly IDataContext _dataContext;
    private readonly IServiceProvider _serviceProvider;

    public Scenario Scenario => Scenario.RefreshToken;

    public RefreshTokenScenarioExecutor(IDataContext dataContext, IServiceProvider serviceProvider)
    {
        _dataContext = dataContext;
        _serviceProvider = serviceProvider;
    }

    public async Task StartScenarioAsync(TelegramChat chat, Update update, CancellationToken ct = default)
    {
        Dictionary<string, string> newArgs = [];
        newArgs[RefreshTokenScenarioArgsKeys.PreviousScenario] = chat.CurrentScenario.ToString()!;
        newArgs[RefreshTokenScenarioArgsKeys.PreviousState] = chat.CurrentState.ToString()!;
        newArgs[RefreshTokenScenarioArgsKeys.PreviousScenarioArgs] =
            JsonSerializer.Serialize(chat.CurrentScenarioArgs, JsonSerializerOptions);

        chat.CurrentScenario = Scenario.RefreshToken;
        chat.CurrentState = (int)RefreshTokenScenarioState.Started;
        chat.CurrentScenarioArgs = newArgs;

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

    public async Task ClearScenarioAsync(TelegramChat chat, CancellationToken ct = default)
    {
        Enum.TryParse<ScenarioEnum>(
            chat.CurrentScenarioArgs[RefreshTokenScenarioArgsKeys.PreviousScenario],
            out var scenario);
        chat.CurrentScenario = scenario;
        chat.CurrentState = int.Parse(chat.CurrentScenarioArgs[RefreshTokenScenarioArgsKeys.PreviousState]);
        chat.CurrentScenarioArgs = JsonSerializer.Deserialize<Dictionary<string, string>>(
            chat.CurrentScenarioArgs[RefreshTokenScenarioArgsKeys.PreviousScenarioArgs]) ?? [];

        await _dataContext.SaveChangesAsync(ct);
    }

    public async Task ChangeStateAsync(TelegramChat chat, IRefreshTokenState newState, Update update,
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
        => ValueTask.FromResult(false);
}