using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using UniCast.Application.Abstractions.Persistence;
using UniCast.Application.Abstractions.Telegram;
using UniCast.Application.TelegramBot.Messages.Scenarios;
using UniCast.Application.TelegramBot.Utils;
using UniCast.Application.TelegramBot.Validation;
using UniCast.Domain.Telegram.Entities;

namespace UniCast.Application.TelegramBot.Scenarios.OrderReference.States;

public sealed class OrderReferenceAskingForPatronymicState : IOrderReferenceState
{
    private const string SkipButtonText = "Пропустить";

    private static readonly InlineKeyboardMarkup SkipPatronymicInputKeyboard = 
        new(InlineKeyboardButton.WithCallbackData(SkipButtonText));

    private static readonly PatronymicValidator PatronymicValidator = new();

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
        await _scenarioExecutor.CancelAndThrowIfCancellationRequestedAsync(chat, update, ct);

        if (!TelegramHelpers.TryGetUserInput(update, out string patronymic))
        {
            await _scenarioExecutor.HandleErrorAsync(
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
            var validationResult = await PatronymicValidator.ValidateAsync(patronymic, ct);
            if (!validationResult.IsValid)
            {
                await _scenarioExecutor.HandleErrorAsync(
                    chat: chat,
                    messageId: TelegramHelpers.GetMessageId(update),
                    errorText: string.Format(
                        OrderReferenceScenarioMessages.InvalidPatronymic, 
                        string.Join('\n', validationResult.Errors.Select(x => $"- <b>{x.ErrorMessage}</b>"))),
                    ct: ct);
                return;
            }

            chat.CurrentScenarioArgs[OrderReferenceScenarioArgsKeys.Patronymic] = patronymic;
        }

        await _dataContext.SaveChangesAsync(ct);

        await _scenarioExecutor.ClearMessagesAsync(
            chat: chat, 
            userMessageId: TelegramHelpers.GetMessageId(update), 
            ct: ct);

        await _scenarioExecutor.ChangeStateAsync(
            chat: chat,
            newState: _scenarioExecutor.GetState((int)OrderReferenceState.AskingForGroup),
            update: update,
            ct: ct);
    }
}