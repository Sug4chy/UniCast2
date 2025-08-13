using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using UniCast.Application.Abstractions.Telegram;
using UniCast.Application.TelegramBot.Messages.Scenarios;
using UniCast.Domain.Telegram.Entities;
using Telegram.Bot.Types.Enums;
using UniCast.Application.Abstractions.Persistence;
using UniCast.Application.TelegramBot.Utils;

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

    public async Task OnStateChangedAsync(TelegramChat chat, Update update, CancellationToken ct = default)
    {
        var message = await _telegramMessageManager.SendMessageAsync(
            chat: chat,
            text: OrderReferenceScenarioMessages.EnterReferencesCount,
            ct: ct);
        chat.CurrentScenarioArgs[OrderReferenceScenarioArgsKeys.MessageToDeleteId] = message.ExtId.ToString();
        await _dataContext.SaveChangesAsync(ct);
    }

    public async Task HandleUserInputAsync(TelegramChat chat, Update update, CancellationToken ct = default)
    {
        await _scenarioExecutor.CancelAndThrowIfCancellationRequestedAsync(chat, update, ct);

        if (update is not { Type: UpdateType.Message, Message: not null, Message.Text: not null } ||
            !int.TryParse(update.Message.Text, out int referencesCount))
        {
            await _scenarioExecutor.HandleErrorAsync(
                chat: chat,
                messageId: TelegramHelpers.GetMessageId(update),
                errorText: OrderReferenceScenarioMessages.InvalidMessageFormat,
                ct: ct);
            return;
        }

        if (referencesCount <= 0)
        {
            await _scenarioExecutor.HandleErrorAsync(
                chat: chat,
                messageId: TelegramHelpers.GetMessageId(update),
                errorText: OrderReferenceScenarioMessages.InvalidNumberFormat,
                ct: ct);
            return;
        }

        chat.CurrentScenarioArgs[OrderReferenceScenarioArgsKeys.ReferencesCount] = referencesCount.ToString();
        await _dataContext.SaveChangesAsync(ct);

        await _scenarioExecutor.ClearMessagesAsync(chat: chat, userMessageId: update.Message.Id, ct: ct);

        await _scenarioExecutor.ChangeStateAsync(
            chat: chat,
            newState: _scenarioExecutor.GetState((int)OrderReferenceState.AskingForReferenceOrderPurpose),
            update: update,
            ct: ct);
    }
}