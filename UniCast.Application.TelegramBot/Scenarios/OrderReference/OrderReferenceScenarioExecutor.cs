using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using UniCast.Application.Abstractions.Persistence;
using UniCast.Application.TelegramBot.Scenarios.OrderReference.States;
using UniCast.Domain.Telegram.Entities;
using UniCast.Domain.Telegram.ValueObjects.Enums;

namespace UniCast.Application.TelegramBot.Scenarios.OrderReference;

public sealed class OrderReferenceScenarioExecutor : IScenarioExecutor<IOrderReferenceState>
{
    private readonly IDataContext _dataContext;
    private readonly IServiceProvider _serviceProvider;

    public Scenario Scenario => Scenario.OrderReference;

    public OrderReferenceScenarioExecutor(IDataContext dataContext, IServiceProvider serviceProvider)
    {
        _dataContext = dataContext;
        _serviceProvider = serviceProvider;
    }

    public async Task StartScenarioAsync(TelegramChat chat, Update update, CancellationToken ct = default)
    {
        chat.CurrentScenario = Scenario.OrderReference;
        chat.CurrentState = (int)OrderReferenceState.Started;

        await _dataContext.SaveChangesAsync(ct);
        await GetState(chat.CurrentState.Value)
            .OnStateChangedAsync(chat, update, ct);
    }

    public IOrderReferenceState GetState(int state)
        => state switch
        {
            (int)OrderReferenceState.Started => new OrderReferenceStartedState(this, _serviceProvider),
            (int)OrderReferenceState.AskingForPatronymic => 
                new OrderReferenceAskingForPatronymicState(this, _serviceProvider),
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
        };

    public async Task ClearScenarioAsync(TelegramChat chat, CancellationToken ct = default)
    {
        chat.CurrentScenario = null;
        chat.CurrentState = null;
        chat.CurrentScenarioArgs = [];

        await _dataContext.SaveChangesAsync(ct);
    }

    public async Task ChangeStateAsync(
        TelegramChat chat,
        IOrderReferenceState newState,
        Update update,
        CancellationToken ct = default)
    {
        chat.CurrentState = GetState(newState);
        await _dataContext.SaveChangesAsync(ct);
        await newState.OnStateChangedAsync(chat, update, ct);
    }

    public int GetState(IOrderReferenceState state)
        => (int)(state switch
        {
            OrderReferenceStartedState => OrderReferenceState.Started,
            OrderReferenceAskingForPatronymicState => OrderReferenceState.AskingForPatronymic,
            _ => throw new ArgumentOutOfRangeException(nameof(state))
        });

    IState IScenarioExecutor.GetState(int state) => GetState(state);

    public ValueTask<bool> CanStartScenarioAsync(Update update, CancellationToken ct = default)
        => ValueTask.FromResult(update.Type is UpdateType.Message &&
                                update.Message!.Text is not null &&
                                update.Message.Text is "/order_reference");
}