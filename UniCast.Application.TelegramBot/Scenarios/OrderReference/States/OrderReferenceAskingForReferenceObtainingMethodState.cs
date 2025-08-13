using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using UniCast.Application.Abstractions.Persistence;
using UniCast.Application.Abstractions.Telegram;
using UniCast.Application.TelegramBot.Messages.Scenarios;
using UniCast.Application.TelegramBot.Utils;
using UniCast.Domain.Telegram.Entities;

namespace UniCast.Application.TelegramBot.Scenarios.OrderReference.States;

public sealed class OrderReferenceAskingForReferenceObtainingMethodState : IOrderReferenceState
{
    private static readonly IEnumerable<string> KeyboardButtonsTexts =
    [
        OrderReferenceScenarioMessages.SelfPickupObtainingMethod,
        OrderReferenceScenarioMessages.SendMeAnEmailObtainingMethod
    ];

    private static readonly InlineKeyboardMarkup ObtainingMethodsKeyboard =
        new(KeyboardButtonsTexts.Select(InlineKeyboardButton.WithCallbackData));

    private readonly OrderReferenceScenarioExecutor _scenarioExecutor;
    private readonly ITelegramMessageManager _telegramMessageManager;
    private readonly IDataContext _dataContext;

    public OrderReferenceAskingForReferenceObtainingMethodState(
        OrderReferenceScenarioExecutor scenarioExecutor,
        IServiceProvider serviceProvider)
    {
        _scenarioExecutor = scenarioExecutor;
        _telegramMessageManager = serviceProvider.GetRequiredService<ITelegramMessageManager>();
        _dataContext = serviceProvider.GetRequiredService<IDataContext>();
    }

    public async Task OnStateChangedAsync(TelegramChat chat, Update update, CancellationToken ct = default)
    {
        var message = await _telegramMessageManager.SendMessageAsync(
            chat: chat,
            text: OrderReferenceScenarioMessages.ChooseReferenceObtainingMethod,
            replyMarkup: ObtainingMethodsKeyboard,
            ct: ct);
        chat.CurrentScenarioArgs[OrderReferenceScenarioArgsKeys.MessageToDeleteId] = message.ExtId.ToString();
        await _dataContext.SaveChangesAsync(ct);
    }

    public async Task HandleUserInputAsync(TelegramChat chat, Update update, CancellationToken ct = default)
    {
        await _scenarioExecutor.CancelAndThrowIfCancellationRequestedAsync(chat, update, ct);

        if (update is not { Type: UpdateType.CallbackQuery, CallbackQuery.Data: not null })
        {
            await _scenarioExecutor.HandleErrorAsync(
                chat: chat,
                messageId: TelegramHelpers.GetMessageId(update),
                errorText: OrderReferenceScenarioMessages.InvalidMessageFormat,
                ct: ct);
            return;
        }

        string obtainingMethod = update.CallbackQuery.Data;
        if (!KeyboardButtonsTexts.Contains(obtainingMethod))
        {
            await _scenarioExecutor.HandleErrorAsync(
                chat: chat,
                messageId: TelegramHelpers.GetMessageId(update),
                errorText: OrderReferenceScenarioMessages.InvalidObtainingMethod,
                ct: ct);
            return;
        }

        switch (obtainingMethod)
        {
            case OrderReferenceScenarioMessages.SelfPickupObtainingMethod:
                chat.CurrentScenarioArgs[OrderReferenceScenarioArgsKeys.ObtainingMethod] = obtainingMethod;
                await _dataContext.SaveChangesAsync(ct);
                await _scenarioExecutor.ClearMessagesAsync(chat: chat, ct: ct);
                await _scenarioExecutor.ChangeStateAsync(
                    chat: chat,
                    newState: _scenarioExecutor.GetState((int)OrderReferenceState.ShowingReferenceFinalVersion),
                    update: update,
                    ct: ct);
                break;
            case OrderReferenceScenarioMessages.SendMeAnEmailObtainingMethod:
                await _scenarioExecutor.ClearMessagesAsync(chat: chat, ct: ct);
                await _scenarioExecutor.ChangeStateAsync(
                    chat: chat,
                    newState: _scenarioExecutor.GetState((int)OrderReferenceState.AskingForEmail),
                    update: update,
                    ct: ct);
                break;
        }
    }
}