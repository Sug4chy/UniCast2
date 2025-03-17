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
    private readonly ITelegramMessageSender _telegramMessageSender;
    private readonly ILogger<RegistrationWaitingForFullNameEnteredState> _logger;

    public RegistrationWaitingForFullNameEnteredState(
        RegistrationScenarioExecutor scenarioExecutor,
        IServiceProvider serviceProvider)
    {
        _scenarioExecutor = scenarioExecutor;
        _telegramMessageSender = serviceProvider.GetRequiredService<ITelegramMessageSender>();
        _logger = serviceProvider.GetRequiredService<ILogger<RegistrationWaitingForFullNameEnteredState>>();
    }

    public Task OnStateChangedAsync(PrivateTelegramChat chat, Update update, CancellationToken ct = default)
        => _telegramMessageSender.SendMessageAsync(
            chatId: chat.ExtId,
            text: "Пожалуйста, введите свои имя и фамилию (именно в этом порядке):",
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

        var fullNameResult = StudentFullName.From(update.Message.Text);
        if (fullNameResult.IsFailure)
        {
            await _telegramMessageSender.SendMessageAsync(
                chatId: chat.ExtId,
                text: "Кажется, вы ввели что-то не то. Пожалуйста, введите своё ФИО ещё раз",
                ct: ct);
            _logger.LogWarning("Invalid user input: '{Input}'. Error: '{Error}'", 
                update.Message.Text, fullNameResult.Error);
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