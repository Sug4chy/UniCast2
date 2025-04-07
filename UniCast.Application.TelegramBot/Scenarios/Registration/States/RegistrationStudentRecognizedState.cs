using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using UniCast.Application.Abstractions.Telegram;
using UniCast.Application.TelegramBot.Messages.Scenarios;
using UniCast.Domain.Telegram.Entities;

namespace UniCast.Application.TelegramBot.Scenarios.Registration.States;

public sealed class RegistrationStudentRecognizedState : IRegistrationState
{
    private const string Yes = "Да";
    private const string No = "Нет";

    private readonly RegistrationScenarioExecutor _scenarioExecutor;
    private readonly ITelegramMessageManager _telegramMessageManager;

    public RegistrationStudentRecognizedState(
        RegistrationScenarioExecutor scenarioExecutor,
        IServiceProvider serviceProvider)
    {
        _scenarioExecutor = scenarioExecutor;
        _telegramMessageManager = serviceProvider.GetRequiredService<ITelegramMessageManager>();
    }

    public Task OnStateChangedAsync(PrivateTelegramChat chat, Update update, CancellationToken ct = default)
        => _telegramMessageManager.SendMessageAsync(
            chatId: chat.ExtId,
            text: string.Format(RegistrationScenarioMessages.ProbablyRecognizeUser,
                chat.CurrentScenarioArgs[RegistrationScenarioArgsKeys.StudentFullName]),
            inlineKeyboard: new InlineKeyboardMarkup((IEnumerable<InlineKeyboardButton>)
            [
                new InlineKeyboardButton(Yes, Yes),
                new InlineKeyboardButton(Yes, Yes)
            ]),
            ct: ct);

    public async Task HandleUserInputAsync(PrivateTelegramChat chat, Update update, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(update.CallbackQuery);

        switch (update.CallbackQuery.Data)
        {
            case Yes:
                await _scenarioExecutor.ChangeStateAsync(
                    chat: chat,
                    newState: _scenarioExecutor.GetState(
                        (int)RegistrationScenarioState.WaitingForMoodlePasswordEntered),
                    update: update,
                    ct: ct);
                break;
            case No:
                await _telegramMessageManager.SendMessageAsync(
                    chatId: chat.ExtId,
                    text: RegistrationScenarioMessages.PleaseReenterUsername,
                    ct: ct);
                await _scenarioExecutor.ChangeStateAsync(
                    chat: chat,
                    newState: _scenarioExecutor.GetState((int)RegistrationScenarioState.MoodleUsernameEntered),
                    update: update,
                    ct: ct);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(update.CallbackQuery));
        }
    }
}