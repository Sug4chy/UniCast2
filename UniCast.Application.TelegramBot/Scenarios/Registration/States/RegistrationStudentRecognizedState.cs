using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using UniCast.Application.Abstractions.Persistence;
using UniCast.Application.Abstractions.Telegram;
using UniCast.Application.TelegramBot.Messages.Scenarios;
using UniCast.Application.TelegramBot.Utils;
using UniCast.Domain.Telegram.Entities;

namespace UniCast.Application.TelegramBot.Scenarios.Registration.States;

public sealed class RegistrationStudentRecognizedState : IRegistrationState
{
    private const string Yes = "Да";
    private const string No = "Нет";

    private static readonly InlineKeyboardMarkup YesOrNoInlineKeyboard = new((IEnumerable<InlineKeyboardButton>)
    [
        new InlineKeyboardButton(Yes, Yes),
        new InlineKeyboardButton(No, No)
    ]);

    private readonly RegistrationScenarioExecutor _scenarioExecutor;
    private readonly ITelegramMessageManager _telegramMessageManager;
    private readonly IDataContext _dataContext;

    public RegistrationStudentRecognizedState(
        RegistrationScenarioExecutor scenarioExecutor,
        IServiceProvider serviceProvider)
    {
        _scenarioExecutor = scenarioExecutor;
        _telegramMessageManager = serviceProvider.GetRequiredService<ITelegramMessageManager>();
        _dataContext = serviceProvider.GetRequiredService<IDataContext>();
    }

    private Task HandleYes(TelegramChat chat, Update update, CancellationToken ct = default)
        => _scenarioExecutor.ChangeStateAsync(
            chat: chat,
            newState: _scenarioExecutor.GetState((int)RegistrationScenarioState.WaitingForMoodlePasswordEntered),
            update: update,
            ct: ct);

    private async Task HandleNo(TelegramChat chat, Update update, CancellationToken ct = default)
    {
        await _telegramMessageManager.DeleteMessageAsync(
            chatId: chat.ExtId,
            messageId: int.Parse(chat.CurrentScenarioArgs[RegistrationScenarioArgsKeys.MessagesToDeleteIds]),
            ct: ct);

        var message = await _telegramMessageManager.SendMessageAsync(
            chat: chat,
            text: RegistrationScenarioMessages.PleaseReenterUsername,
            ct: ct);
        chat.CurrentScenarioArgs[RegistrationScenarioArgsKeys.MessagesToDeleteIds] = message.ExtId.ToString();
        await _dataContext.SaveChangesAsync(ct);

        await _scenarioExecutor.ChangeStateAsync(
            chat: chat,
            newState: _scenarioExecutor.GetState((int)RegistrationScenarioState.MoodleUsernameEntered),
            update: update,
            ct: ct);
    }

    public async Task OnStateChangedAsync(TelegramChat chat, Update update, CancellationToken ct = default)
    {
        await _telegramMessageManager.DeleteMessageAsync(
            chatId: chat.ExtId,
            messageId: int.Parse(chat.CurrentScenarioArgs[RegistrationScenarioArgsKeys.MessagesToDeleteIds]),
            ct: ct);
        await _telegramMessageManager.DeleteMessageAsync(
            chatId: chat.ExtId,
            messageId: TelegramHelpers.GetMessageId(update),
            ct: ct);

        var message = await _telegramMessageManager.SendMessageAsync(
            chat: chat,
            text: string.Format(RegistrationScenarioMessages.ProbablyRecognizeUser,
                chat.CurrentScenarioArgs[RegistrationScenarioArgsKeys.StudentFullName]),
            replyMarkup: YesOrNoInlineKeyboard,
            ct: ct);
        chat.CurrentScenarioArgs[RegistrationScenarioArgsKeys.MessagesToDeleteIds] = message.ExtId.ToString();
        await _dataContext.SaveChangesAsync(ct);
    }

    public async Task HandleUserInputAsync(TelegramChat chat, Update update, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(update.CallbackQuery);

        switch (update.CallbackQuery.Data)
        {
            case Yes:
                await HandleYes(chat, update, ct);
                break;
            case No:
                await HandleNo(chat, update, ct);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(update.CallbackQuery));
        }
    }
}