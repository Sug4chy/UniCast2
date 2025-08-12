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

    private async Task HandleErrorAsync(
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

        var message = await _telegramMessageManager.SendMessageAsync(
            chat: chat,
            text: errorText,
            replyMarkup: ObtainingMethodsKeyboard,
            ct: ct);
        chat.CurrentScenarioArgs[OrderReferenceScenarioArgsKeys.MessageToDeleteId] = message.ExtId.ToString();
        await _dataContext.SaveChangesAsync(ct);
    }

    private Task ClearStateMessagesAsync(TelegramChat chat, CancellationToken ct = default)
        => _telegramMessageManager.DeleteMessageAsync(
            chatId: chat.ExtId,
            messageId: int.Parse(chat.CurrentScenarioArgs[OrderReferenceScenarioArgsKeys.MessageToDeleteId]),
            ct: ct);

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
            await HandleErrorAsync(
                chat: chat,
                messageId: TelegramHelpers.GetMessageId(update),
                errorText: OrderReferenceScenarioMessages.InvalidMessageFormat,
                ct: ct);
            return;
        }

        string obtainingMethod = update.CallbackQuery.Data;
        if (!KeyboardButtonsTexts.Contains(obtainingMethod))
        {
            await HandleErrorAsync(
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
                await ClearStateMessagesAsync(chat, ct);
                await _scenarioExecutor.ChangeStateAsync(
                    chat: chat,
                    newState: _scenarioExecutor.GetState((int)OrderReferenceState.ShowingReferenceFinalVersion),
                    update: update,
                    ct: ct);
                break;
            case OrderReferenceScenarioMessages.SendMeAnEmailObtainingMethod:
                await ClearStateMessagesAsync(chat, ct);
                await _scenarioExecutor.ChangeStateAsync(
                    chat: chat,
                    newState: _scenarioExecutor.GetState((int)OrderReferenceState.AskingForEmail),
                    update: update,
                    ct: ct);
                break;
        }
    }
}