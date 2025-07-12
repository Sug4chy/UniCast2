using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using UniCast.Application.Abstractions.Persistence;
using UniCast.Application.Abstractions.Telegram;
using UniCast.Application.TelegramBot.Messages.Scenarios;
using UniCast.Domain.Telegram.Entities;

namespace UniCast.Application.TelegramBot.Scenarios.OrderReference.States;

public sealed class OrderReferenceOtherOrderPurposeSelectedState : IOrderReferenceState
{
    private readonly OrderReferenceScenarioExecutor _scenarioExecutor;
    private readonly ITelegramMessageManager _telegramMessageManager;
    private readonly IDataContext _dataContext;

    public OrderReferenceOtherOrderPurposeSelectedState(
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
            text: OrderReferenceScenarioMessages.EnterYourOrderPurpose,
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

        chat.CurrentScenarioArgs[OrderReferenceScenarioArgsKeys.OrderPurpose] = update.Message.Text;
        await _dataContext.SaveChangesAsync(ct);

        await _scenarioExecutor.ChangeStateAsync(
            chat: chat,
            newState: _scenarioExecutor.GetState((int)OrderReferenceState.AskingForReferenceObtainingMethod),
            update: update,
            ct: ct);
    }
}