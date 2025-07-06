using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using UniCast.Application.Abstractions.Telegram;
using UniCast.Application.TelegramBot.Messages.Scenarios;
using UniCast.Domain.Telegram.Entities;
using Telegram.Bot.Types.Enums;
using UniCast.Application.Abstractions.Persistence;

namespace UniCast.Application.TelegramBot.Scenarios.OrderReference.States;

public sealed class OrderReferenceAskingForReferencesCountState : IOrderReferenceState
{
    private readonly OrderReferenceScenarioExecutor _scenarioExecutor;
    private readonly ITelegramMessageManager _telegramMessageManager;
    private readonly IDataContext _dataContext;

    public OrderReferenceAskingForReferencesCountState(
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
            text: OrderReferenceScenarioMessages.EnterReferencesCount,
            ct: ct);

    public async Task HandleUserInputAsync(TelegramChat chat, Update update, CancellationToken ct = default)
    {
        if (update is not { Type: UpdateType.Message, Message: not null, Message.Text: not null } ||
            !int.TryParse(update.Message.Text, out int referencesCount))
        {
            await _telegramMessageManager.SendMessageAsync(
                chatId: chat.ExtId,
                text: OrderReferenceScenarioMessages.InvalidMessageFormat,
                ct: ct);
            return;
        }

        if (referencesCount <= 0)
        {
            await _telegramMessageManager.SendMessageAsync(
                chatId: chat.ExtId,
                text: OrderReferenceScenarioMessages.InvalidNumberFormat,
                ct: ct);
            return;
        }

        chat.CurrentScenarioArgs[OrderReferenceScenarioArgsKeys.ReferencesCount] = referencesCount.ToString();
        await _dataContext.SaveChangesAsync(ct);

        await _scenarioExecutor.ChangeStateAsync(
            chat: chat,
            newState: _scenarioExecutor.GetState((int)OrderReferenceState.AskingForReferenceOrderPurpose),
            update: update,
            ct: ct);
    }
}