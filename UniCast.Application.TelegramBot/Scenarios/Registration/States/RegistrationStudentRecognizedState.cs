using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using UniCast.Application.Abstractions.Telegram;
using UniCast.Domain.Telegram.Entities;

namespace UniCast.Application.TelegramBot.Scenarios.Registration.States;

public sealed class RegistrationStudentRecognizedState : IRegistrationState
{
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
            text: $"Кажется, я тебя узнал! Ты же {chat.CurrentScenarioArgs["STUDENT_FULL_NAME"]}, верно?",
            inlineKeyboard: new InlineKeyboardMarkup((IEnumerable<InlineKeyboardButton>)
            [
                new InlineKeyboardButton("Да", "Да"),
                new InlineKeyboardButton("Нет", "Нет")
            ]),
            ct: ct);

    public async Task HandleUserInputAsync(PrivateTelegramChat chat, Update update, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(update.CallbackQuery);

        switch (update.CallbackQuery.Data)
        {
            case "Да":
                await _scenarioExecutor.ChangeStateAsync(
                    chat: chat,
                    newState: _scenarioExecutor.GetState(
                        (int)RegistrationScenarioState.WaitingForMoodlePasswordEntered),
                    update: update,
                    ct: ct);
                break;
            case "Нет":
                await _telegramMessageManager.SendMessageAsync(
                    chatId: chat.ExtId,
                    text: "Извините за эту промашку. Пожалуйста, введите свой login ещё раз",
                    ct: ct);
                await _scenarioExecutor.ChangeStateAsync(
                    chat: chat,
                    newState: _scenarioExecutor.GetState((int)RegistrationScenarioState.MoodleLoginEntered),
                    update: update,
                    ct: ct);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(update.CallbackQuery));
        }
    }
}