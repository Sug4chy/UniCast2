using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using UniCast.Application.Abstractions.Persistence;
using UniCast.Application.Abstractions.Telegram;
using UniCast.Application.TelegramBot.Messages.Scenarios;
using UniCast.Application.TelegramBot.Utils;
using UniCast.Domain.Telegram.Entities;

namespace UniCast.Application.TelegramBot.Scenarios.OrderReference.States;

public sealed class OrderReferenceAskingForPatronymicState : IOrderReferenceState
{
    private const string SkipButtonText = "Пропустить";

    private static readonly InlineKeyboardMarkup SkipPatronymicInputKeyboard = 
        new(InlineKeyboardButton.WithCallbackData(SkipButtonText));

    private readonly OrderReferenceScenarioExecutor _scenarioExecutor;
    private readonly ITelegramMessageManager _telegramMessageManager;
    private readonly IDataContext _dataContext;

    public OrderReferenceAskingForPatronymicState(
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
        await _telegramMessageManager.DeleteMessageAsync(chat.ExtId, messageId, ct);

        var message = await _telegramMessageManager.SendMessageAsync(
            chat: chat,
            text: errorText,
            replyMarkup: SkipPatronymicInputKeyboard,
            ct: ct);
        chat.CurrentScenarioArgs[OrderReferenceScenarioArgsKeys.MessageToDeleteId] = message.ExtId.ToString();
        await _dataContext.SaveChangesAsync(ct);
    }

    private async Task ClearStateMessagesAsync(TelegramChat chat, int userMessageId, CancellationToken ct = default)
    {
        int messageToDeleteId = int.Parse(chat.CurrentScenarioArgs[OrderReferenceScenarioArgsKeys.MessageToDeleteId]);
        await _telegramMessageManager.DeleteMessageAsync(
            chatId: chat.ExtId,
            messageId: messageToDeleteId,
            ct: ct);
        if (messageToDeleteId != userMessageId)
        {
            await _telegramMessageManager.DeleteMessageAsync(chat.ExtId, userMessageId, ct);
        }
    }

    public async Task OnStateChangedAsync(TelegramChat chat, Update update, CancellationToken ct = default)
    {
        var message = await _telegramMessageManager.SendMessageAsync(
            chat: chat,
            text: OrderReferenceScenarioMessages.EnterPatronymic,
            replyMarkup: SkipPatronymicInputKeyboard,
            ct: ct);
        chat.CurrentScenarioArgs[OrderReferenceScenarioArgsKeys.MessageToDeleteId] = message.ExtId.ToString();
        await _dataContext.SaveChangesAsync(ct);
    }

    public async Task HandleUserInputAsync(TelegramChat chat, Update update, CancellationToken ct = default)
    {
        if (!TelegramHelpers.TryGetUserInput(update, out string patronymic))
        {
            await HandleErrorAsync(
                chat: chat,
                messageId: TelegramHelpers.GetMessageId(update),
                errorText: OrderReferenceScenarioMessages.InvalidMessageFormat,
                ct: ct);
            return;
        }

        if (patronymic == SkipButtonText)
        {
            chat.CurrentScenarioArgs[OrderReferenceScenarioArgsKeys.Patronymic] = string.Empty;
        }
        else
        {
            if (!PatronymicValidator.Validate(patronymic))
            {
                await HandleErrorAsync(
                    chat: chat,
                    messageId: TelegramHelpers.GetMessageId(update),
                    errorText: OrderReferenceScenarioMessages.InvalidPatronymic,
                    ct: ct);
                return;
            }

            chat.CurrentScenarioArgs[OrderReferenceScenarioArgsKeys.Patronymic] = patronymic;
        }

        await _dataContext.SaveChangesAsync(ct);

        await ClearStateMessagesAsync(chat: chat, userMessageId: TelegramHelpers.GetMessageId(update), ct: ct);

        await _scenarioExecutor.ChangeStateAsync(
            chat: chat,
            newState: _scenarioExecutor.GetState((int)OrderReferenceState.AskingForGroup),
            update: update,
            ct: ct);
    }
}