using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using UniCast.Application.Abstractions.Telegram;
using UniCast.Application.TelegramBot.Messages.Scenarios;
using UniCast.Domain.Telegram.Entities;
using UniCast.Domain.Telegram.ValueObjects.Enums;

namespace UniCast.Application.TelegramBot.Scenarios.OrderReference.States;

public sealed class OrderReferenceStartedState : IOrderReferenceState
{
    private readonly OrderReferenceScenarioExecutor _scenarioExecutor;
    private readonly ITelegramMessageManager _telegramMessageManager;
    private readonly ILogger<OrderReferenceStartedState> _logger;

    public OrderReferenceStartedState(
        OrderReferenceScenarioExecutor scenarioExecutor,
        IServiceProvider serviceProvider)
    {
        _scenarioExecutor = scenarioExecutor;
        _telegramMessageManager = serviceProvider.GetRequiredService<ITelegramMessageManager>();
        _logger = serviceProvider.GetRequiredService<ILogger<OrderReferenceStartedState>>();
    }

    public async Task OnStateChangedAsync(TelegramChat chat, Update update, CancellationToken ct = default)
    {
        _logger.LogInformation("Started {ScenarioName} for chat {ChatID}", 
            nameof(Scenario.OrderReference), chat.Id);

        await _telegramMessageManager.DeleteMessageAsync(chatId: chat.ExtId, messageId: update.Message!.Id, ct: ct);
        await _telegramMessageManager.SendMessageAsync(
            chatId: chat.ExtId,
            text: OrderReferenceScenarioMessages.Introduction,
            ct: ct);

        await _scenarioExecutor.ChangeStateAsync(
            chat: chat,
            newState: _scenarioExecutor.GetState((int)OrderReferenceState.AskingForPatronymic),
            update: update,
            ct: ct);
    }

    public Task HandleUserInputAsync(TelegramChat chat, Update update, CancellationToken ct = default)
        => Task.CompletedTask;
}