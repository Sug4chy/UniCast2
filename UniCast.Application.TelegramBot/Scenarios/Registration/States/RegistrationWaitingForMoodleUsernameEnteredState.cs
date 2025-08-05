using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using UniCast.Application.Abstractions.Persistence;
using UniCast.Application.Abstractions.Telegram;
using UniCast.Application.TelegramBot.Messages.Scenarios;
using UniCast.Application.TelegramBot.Utils;
using UniCast.Domain.Moodle;
using UniCast.Domain.Telegram.Entities;

namespace UniCast.Application.TelegramBot.Scenarios.Registration.States;

public sealed class RegistrationWaitingForMoodleUsernameEnteredState : IRegistrationState
{
    private readonly RegistrationScenarioExecutor _scenarioExecutor;
    private readonly ITelegramMessageManager _telegramMessageManager;
    private readonly IDataContext _dataContext;

    public RegistrationWaitingForMoodleUsernameEnteredState(
        RegistrationScenarioExecutor scenarioExecutor,
        IServiceProvider serviceProvider)
    {
        _scenarioExecutor = scenarioExecutor;
        _telegramMessageManager = serviceProvider.GetRequiredService<ITelegramMessageManager>();
        _dataContext = serviceProvider.GetRequiredService<IDataContext>();
    }

    public async Task OnStateChangedAsync(TelegramChat chat, Update update, CancellationToken ct = default)
    {
        var message = await _telegramMessageManager.SendMessageAsync(
            chat: chat,
            text: RegistrationScenarioMessages.EnterUsername,
            ct: ct);

        chat.CurrentScenarioArgs[RegistrationScenarioArgsKeys.MessagesToDeleteIds] = message.ExtId.ToString();
        await _dataContext.SaveChangesAsync(ct);
    }

    public async Task HandleUserInputAsync(TelegramChat chat, Update update, CancellationToken ct = default)
    {
        if (update is { Type: UpdateType.Message, Message.PinnedMessage: not null })
        {
            await _telegramMessageManager.DeleteMessageAsync(chatId: chat.ExtId, messageId: update.Message.Id, ct: ct);
            return;
        }

        if (update is not { Type: UpdateType.Message, Message.Text: not null })
        {
            await HandleError(
                chat: chat,
                messageId: TelegramHelpers.GetMessageId(update),
                errorText: RegistrationScenarioMessages.InvalidUsernameMessageFormat,
                ct: ct);
            return;
        }

        var moodleAccount = await GetMoodleAccountAsync(update.Message.Text, ct);
        if (moodleAccount is null)
        {
            await HandleError(
                chat: chat,
                messageId: TelegramHelpers.GetMessageId(update),
                errorText: RegistrationScenarioMessages.CantRecognizeUserByUsername,
                ct: ct);
            return;
        }

        chat.CurrentScenarioArgs[RegistrationScenarioArgsKeys.StudentFullName] = moodleAccount.Student!.FullName;
        chat.CurrentScenarioArgs[RegistrationScenarioArgsKeys.MoodleUsername] = moodleAccount.Username;
        await _scenarioExecutor.ChangeStateAsync(
            chat: chat,
            newState: _scenarioExecutor.GetState((int)RegistrationScenarioState.StudentRecognized),
            update: update,
            ct: ct);
    }

    private async Task HandleError(TelegramChat chat, int messageId, string errorText, CancellationToken ct = default)
    {
        int botsPrevMessageId = int.Parse(chat.CurrentScenarioArgs[RegistrationScenarioArgsKeys.MessagesToDeleteIds]);
        await _telegramMessageManager.DeleteMessageAsync(chat.ExtId, botsPrevMessageId, ct);
        await _telegramMessageManager.DeleteMessageAsync(chat.ExtId, messageId, ct);

        var message = await _telegramMessageManager.SendMessageAsync(chat: chat, text: errorText, ct: ct);
        chat.CurrentScenarioArgs[RegistrationScenarioArgsKeys.MessagesToDeleteIds] = message.ExtId.ToString();
        await _dataContext.SaveChangesAsync(ct);
    }

    private Task<MoodleAccount?> GetMoodleAccountAsync(string username, CancellationToken ct = default)
        => _dataContext.MoodleAccounts
            .Include(x => x.Student)
            .SingleOrDefaultAsync(x => x.Username == username, ct);
}