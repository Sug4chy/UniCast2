using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using UniCast.Application.Abstractions.Telegram;
using UniCast.Application.TelegramBot.Messages.Scenarios;
using UniCast.Domain.Telegram.Entities;

namespace UniCast.Application.TelegramBot.Scenarios.Registration.States;

public sealed class RegistrationWaitingForMoodleUsernameEnteredState : IRegistrationState
{
    private readonly RegistrationScenarioExecutor _scenarioExecutor;
    private readonly ITelegramMessageManager _telegramMessageManager;

    public RegistrationWaitingForMoodleUsernameEnteredState(
        RegistrationScenarioExecutor scenarioExecutor,
        IServiceProvider serviceProvider)
    {
        _scenarioExecutor = scenarioExecutor;
        _telegramMessageManager = serviceProvider.GetRequiredService<ITelegramMessageManager>();
    }

    public async Task OnStateChangedAsync(PrivateTelegramChat chat, Update update, CancellationToken ct = default)
    {
        await _telegramMessageManager.SendMessageAsync(
            chatId: chat.ExtId,
            text: RegistrationScenarioMessages.EnterUsername,
            ct: ct);

        await _scenarioExecutor.ChangeStateAsync(
            chat: chat,
            newState: _scenarioExecutor.GetState((int)RegistrationScenarioState.MoodleUsernameEntered),
            update: update,
            ct: ct);
    }

    public Task HandleUserInputAsync(PrivateTelegramChat chat, Update update, CancellationToken ct = default)
        => Task.CompletedTask;
}