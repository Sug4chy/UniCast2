using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using UniCast.Application.Abstractions.Telegram;
using UniCast.Domain.Telegram.Entities;

namespace UniCast.Application.TelegramBot.Scenarios.Registration.States;

public sealed class RegistrationStartedState : IRegistrationState
{
    private readonly RegistrationScenarioExecutor _scenarioExecutor;
    private readonly ITelegramMessageSender _telegramMessageSender;

    public RegistrationStartedState(
        RegistrationScenarioExecutor scenarioExecutor,
        IServiceProvider serviceProvider)
    {
        _scenarioExecutor = scenarioExecutor;
        _telegramMessageSender = serviceProvider.GetRequiredService<ITelegramMessageSender>();
    }

    public async Task OnStateChangedAsync(PrivateTelegramChat chat, Update update, CancellationToken ct = default)
    {
        await _telegramMessageSender.SendMessageAsync(
            chatId: chat.ExtId, 
            text: "Здравствуйте! Я - бот, цель жизни которого, это передавать студентам информацию. " +
            "Давайте начнём знакомиться!", 
            ct: ct);

        await _scenarioExecutor.ChangeStateAsync(
            chat,
            _scenarioExecutor.GetState((int)RegistrationScenarioState.WaitingForFullNameEntered),
            update, ct);
    }

    public Task HandleUserInputAsync(PrivateTelegramChat chat, Update update, CancellationToken ct = default)
        => Task.CompletedTask;
}