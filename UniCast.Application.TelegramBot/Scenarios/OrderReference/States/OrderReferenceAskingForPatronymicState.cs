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

public sealed class OrderReferenceAskingForPatronymicState : IOrderReferenceState
{
    private const string SkipButtonText = "Пропустить";

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

    public Task OnStateChangedAsync(TelegramChat chat, Update update, CancellationToken ct = default)
        => _telegramMessageManager.SendMessageAsync(
            chatId: chat.ExtId,
            text: OrderReferenceScenarioMessages.EnterPatronymic,
            replyMarkup: new ReplyKeyboardMarkup(new KeyboardButton(SkipButtonText)) { ResizeKeyboard = true },
            ct: ct);

    public async Task HandleUserInputAsync(TelegramChat chat, Update update, CancellationToken ct = default)
    {
        if (update is not { Type: UpdateType.Message, Message: not null, Message.Text: not null })
        {
            await _telegramMessageManager.SendMessageAsync(
                chatId: chat.ExtId,
                text: OrderReferenceScenarioMessages.InvalidMessageFormat,
                ct: ct);
            return;
        }

        if (update.Message.Text == SkipButtonText)
        {
            chat.CurrentScenarioArgs[OrderReferenceScenarioArgsKeys.Patronymic] = string.Empty;
        }
        else
        {
            if (!PatronymicValidator.Validate(update.Message.Text))
            {
                await _telegramMessageManager.SendMessageAsync(
                    chatId: chat.ExtId,
                    text: OrderReferenceScenarioMessages.InvalidPatronymic,
                    ct: ct);
                return;
            }

            chat.CurrentScenarioArgs[OrderReferenceScenarioArgsKeys.Patronymic] = update.Message.Text;
        }

        await _dataContext.SaveChangesAsync(ct);

        await _scenarioExecutor.ChangeStateAsync(
            chat: chat,
            newState: _scenarioExecutor.GetState((int)OrderReferenceState.AskingForGroup),
            update: update,
            ct: ct);
    }
}