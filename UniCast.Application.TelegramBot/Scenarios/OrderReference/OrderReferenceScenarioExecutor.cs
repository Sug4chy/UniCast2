using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using UniCast.Application.Abstractions.Persistence;
using UniCast.Application.Abstractions.Telegram;
using UniCast.Application.TelegramBot.Exceptions;
using UniCast.Application.TelegramBot.Messages.Scenarios;
using UniCast.Application.TelegramBot.Scenarios.OrderReference.States;
using UniCast.Application.TelegramBot.Utils;
using UniCast.Domain.Telegram.Entities;
using UniCast.Domain.Telegram.ValueObjects.Enums;

namespace UniCast.Application.TelegramBot.Scenarios.OrderReference;

public sealed class OrderReferenceScenarioExecutor : IScenarioExecutor<IOrderReferenceState>
{
    private readonly IDataContext _dataContext;
    private readonly IServiceProvider _serviceProvider;
    private readonly ITelegramMessageManager _telegramMessageManager;

    public Scenario Scenario => Scenario.OrderReference;

    public OrderReferenceScenarioExecutor(IDataContext dataContext, IServiceProvider serviceProvider)
    {
        _dataContext = dataContext;
        _serviceProvider = serviceProvider;
        _telegramMessageManager = serviceProvider.GetRequiredService<ITelegramMessageManager>();
    }

    private async Task ClearScenarioMessagesAsync(TelegramChat chat, int userMessageId, CancellationToken ct = default)
    {
        int botsPrevMessageId = int.Parse(chat.CurrentScenarioArgs[OrderReferenceScenarioArgsKeys.MessageToDeleteId]);
        await _telegramMessageManager.DeleteMessageAsync(
            chatId: chat.ExtId,
            messageId: botsPrevMessageId,
            ct: ct);
        if (userMessageId != botsPrevMessageId)
        {
            await _telegramMessageManager.DeleteMessageAsync(chat.ExtId, userMessageId, ct);
        }
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
            (int)OrderReferenceState.AskingForGroup => new OrderReferenceAskingForGroupState(this, _serviceProvider),
            (int)OrderReferenceState.AskingForReferencesCount =>
                new OrderReferenceAskingForReferencesCountState(this, _serviceProvider),
            (int)OrderReferenceState.AskingForReferenceOrderPurpose =>
                new OrderReferenceAskingForReferenceOrderPurposeState(this, _serviceProvider),
            (int)OrderReferenceState.OtherOrderPurposeSelected =>
                new OrderReferenceOtherOrderPurposeSelectedState(this, _serviceProvider),
            (int)OrderReferenceState.AskingForReferenceObtainingMethod =>
                new OrderReferenceAskingForReferenceObtainingMethodState(this, _serviceProvider),
            (int)OrderReferenceState.AskingForEmail => new OrderReferenceAskingForEmailState(this, _serviceProvider),
            (int)OrderReferenceState.ShowingReferenceFinalVersion =>
                new OrderReferenceShowingReferenceFinalVersionState(this, _serviceProvider),
            (int)OrderReferenceState.Completed => new OrderReferenceCompletedState(this, _serviceProvider),
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
            OrderReferenceAskingForGroupState => OrderReferenceState.AskingForGroup,
            OrderReferenceAskingForReferencesCountState => OrderReferenceState.AskingForReferencesCount,
            OrderReferenceAskingForReferenceOrderPurposeState => OrderReferenceState.AskingForReferenceOrderPurpose,
            OrderReferenceOtherOrderPurposeSelectedState => OrderReferenceState.OtherOrderPurposeSelected,
            OrderReferenceAskingForReferenceObtainingMethodState =>
                OrderReferenceState.AskingForReferenceObtainingMethod,
            OrderReferenceAskingForEmailState => OrderReferenceState.AskingForEmail,
            OrderReferenceShowingReferenceFinalVersionState => OrderReferenceState.ShowingReferenceFinalVersion,
            OrderReferenceCompletedState => OrderReferenceState.Completed,
            _ => throw new ArgumentOutOfRangeException(nameof(state))
        });

    IState IScenarioExecutor.GetState(int state) => GetState(state);

    public ValueTask<bool> CanStartScenarioAsync(Update update, CancellationToken ct = default)
        => ValueTask.FromResult(update.Type is UpdateType.Message &&
                                update.Message!.Text is not null &&
                                update.Message.Text is "/order_reference");

    public async Task ClearMessagesAsync(TelegramChat chat, int? userMessageId = null, CancellationToken ct = default)
    {
        int botsPrevMessageId = int.Parse(chat.CurrentScenarioArgs[OrderReferenceScenarioArgsKeys.MessageToDeleteId]);
        await _telegramMessageManager.DeleteMessageAsync(
            chatId: chat.ExtId,
            messageId: botsPrevMessageId,
            ct: ct);
        if (userMessageId.HasValue && userMessageId.Value != botsPrevMessageId)
        {
            await _telegramMessageManager.DeleteMessageAsync(chat.ExtId, userMessageId.Value, ct);
        }
    }

    public async Task CancelAndThrowIfCancellationRequestedAsync(
        TelegramChat chat,
        Update update,
        CancellationToken ct = default)
    {
        if (update is not { Type: UpdateType.Message, Message.Text: "Отмена" })
        {
            return;
        }

        await ClearScenarioMessagesAsync(chat, TelegramHelpers.GetMessageId(update), ct);
        await ClearScenarioAsync(chat, ct);
        await _telegramMessageManager.SendMessageAsync(
            chatId: chat.ExtId,
            text: OrderReferenceScenarioMessages.Cancelled,
            replyMarkup: new ReplyKeyboardRemove(),
            ct: ct);

        throw new ScenarioCancelledException(nameof(Scenario.OrderReference));
    }

    public async Task HandleErrorAsync(
        TelegramChat chat,
        int messageId,
        string errorText,
        CancellationToken ct = default)
    {
        int botsPrevMessageId = int.Parse(chat.CurrentScenarioArgs[OrderReferenceScenarioArgsKeys.MessageToDeleteId]);
        await _telegramMessageManager.DeleteMessageAsync(chat.ExtId, botsPrevMessageId, ct);
        if (botsPrevMessageId != messageId)
        {
            await _telegramMessageManager.DeleteMessageAsync(chat.ExtId, messageId, ct);
        }

        var message = await _telegramMessageManager.SendMessageAsync(chat: chat, text: errorText, ct: ct);
        chat.CurrentScenarioArgs[OrderReferenceScenarioArgsKeys.MessageToDeleteId] = message.ExtId.ToString();
        await _dataContext.SaveChangesAsync(ct);
    }
}