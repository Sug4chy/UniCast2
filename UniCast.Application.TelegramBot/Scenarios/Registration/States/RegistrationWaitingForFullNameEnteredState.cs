using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using UniCast.Application.Abstractions.Telegram;
using UniCast.Domain.Students.ValueObjects;
using UniCast.Domain.Telegram.Entities;

namespace UniCast.Application.TelegramBot.Scenarios.Registration.States;

public sealed class RegistrationWaitingForFullNameEnteredState : IRegistrationState
{
    private readonly RegistrationScenarioExecutor _scenarioExecutor;
    private readonly ITelegramMessageManager _telegramMessageManager;
    private readonly ILogger<RegistrationWaitingForFullNameEnteredState> _logger;

    public RegistrationWaitingForFullNameEnteredState(
        RegistrationScenarioExecutor scenarioExecutor,
        IServiceProvider serviceProvider)
    {
        _scenarioExecutor = scenarioExecutor;
        _telegramMessageManager = serviceProvider.GetRequiredService<ITelegramMessageManager>();
        _logger = serviceProvider.GetRequiredService<ILogger<RegistrationWaitingForFullNameEnteredState>>();
    }

    public Task OnStateChangedAsync(PrivateTelegramChat chat, Update update, CancellationToken ct = default)
        => _telegramMessageManager.SendMessageAsync(
            chatId: chat.ExtId,
            text: "Давайте начнём знакомство. Пожалуйста, введите своё ФИО",
            ct: ct
        );

    public async Task HandleUserInputAsync(PrivateTelegramChat chat, Update update, CancellationToken ct = default)
    {
        if (update.Type is not UpdateType.Message || update.Message!.Text is null)
        {
            _logger.LogError("Update with ID {UpdateID} is invalid in {StateName}", 
                update.Id, nameof(RegistrationWaitingForFullNameEnteredState));
            return;
        }

        if (!StudentFullName.IsValid(update.Message.Text))
        {
            await _telegramMessageManager.SendMessageAsync(
                chatId: chat.ExtId,
                text: "Кажется, вы ввели что-то не то. Пожалуйста, введите своё ФИО ещё раз",
                ct: ct);
            _logger.LogWarning("Invalid user input: '{Input}'", 
                update.Message.Text);
            return;
        }

        string fullName = update.Message.Text;
        chat.CurrentScenarioArgs["FULL_NAME"] = fullName;

        await _scenarioExecutor.ChangeStateAsync(
            chat,
            _scenarioExecutor.GetState((int)RegistrationScenarioState.WaitingForGroupNameEntered),
            update, ct);
    }
}