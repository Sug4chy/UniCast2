using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using UniCast.Application.Abstractions.Telegram;
using UniCast.Application.TelegramBot.Messages.Scenarios;
using UniCast.Domain.Telegram.Entities;

namespace UniCast.Application.TelegramBot.Scenarios.Registration.States;

public sealed class RegistrationStartedState : IRegistrationState
{
    private readonly RegistrationScenarioExecutor _scenarioExecutor;
    private readonly ITelegramMessageManager _telegramMessageManager;

    public RegistrationStartedState(
        RegistrationScenarioExecutor scenarioExecutor,
        IServiceProvider serviceProvider)
    {
        _scenarioExecutor = scenarioExecutor;
        _telegramMessageManager = serviceProvider.GetRequiredService<ITelegramMessageManager>();
    }

    public async Task OnStateChangedAsync(TelegramChat chat, Update update, CancellationToken ct = default)
    {
        await _telegramMessageManager.DeleteMessageAsync(chatId: chat.ExtId, messageId: update.Message!.Id, ct: ct);

        var message = await _telegramMessageManager.SendMessageAsync(
            chat: chat, 
            text: RegistrationScenarioMessages.Greeting, 
            ct: ct);
        await _telegramMessageManager.PinMessageAsync(chatId: chat.ExtId, messageId: message.ExtId, ct: ct);

        await _scenarioExecutor.ChangeStateAsync(
            chat,
            _scenarioExecutor.GetState((int)RegistrationScenarioState.WaitingForMoodleUsernameEntered),
            update, ct);
    }

    public Task HandleUserInputAsync(TelegramChat chat, Update update, CancellationToken ct = default)
        => Task.CompletedTask;
}