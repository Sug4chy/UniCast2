using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using UniCast.Application.Abstractions.Persistence;
using UniCast.Application.Abstractions.Telegram;
using UniCast.Application.TelegramBot.Messages.Scenarios;
using UniCast.Domain.Moodle;
using UniCast.Domain.Telegram.Entities;

namespace UniCast.Application.TelegramBot.Scenarios.Registration.States;

public sealed class RegistrationMoodleUsernameEnteredState : IRegistrationState
{
    private readonly RegistrationScenarioExecutor _scenarioExecutor;
    private readonly ITelegramMessageManager _telegramMessageManager;
    private readonly IDataContext _dataContext;

    public RegistrationMoodleUsernameEnteredState(
        RegistrationScenarioExecutor scenarioExecutor,
        IServiceProvider serviceProvider)
    {
        _scenarioExecutor = scenarioExecutor;
        _telegramMessageManager = serviceProvider.GetRequiredService<ITelegramMessageManager>();
        _dataContext = serviceProvider.GetRequiredService<IDataContext>();
    }

    public Task OnStateChangedAsync(PrivateTelegramChat chat, Update update, CancellationToken ct = default)
        => Task.CompletedTask;

    public async Task HandleUserInputAsync(PrivateTelegramChat chat, Update update, CancellationToken ct = default)
    {
        if (update.Message!.Text is null)
        {
            await SendError(chat.ExtId, RegistrationScenarioMessages.PleaseEnterUsername, ct);
            return;
        }

        var moodleAccount = await GetMoodleAccountAsync(update.Message.Text, ct);
        if (moodleAccount is null)
        {
            await SendError(chat.ExtId, RegistrationScenarioMessages.CantRecognizeUserByUsername, ct);
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

    private Task SendError(long chatId, string errorText, CancellationToken ct = default)
        => _telegramMessageManager.SendMessageAsync(
            chatId: chatId,
            text: errorText,
            ct: ct);

    private Task<MoodleAccount?> GetMoodleAccountAsync(string username, CancellationToken ct = default)
        => _dataContext.MoodleAccounts
            .Include(x => x.Student)
            .SingleOrDefaultAsync(x => x.Username == username, ct);
}