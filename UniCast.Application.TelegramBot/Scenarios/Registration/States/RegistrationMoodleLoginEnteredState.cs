using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using UniCast.Application.Abstractions.Persistence;
using UniCast.Application.Abstractions.Telegram;
using UniCast.Domain.Moodle;
using UniCast.Domain.Telegram.Entities;

namespace UniCast.Application.TelegramBot.Scenarios.Registration.States;

public sealed class RegistrationMoodleLoginEnteredState : IRegistrationState
{
    private readonly RegistrationScenarioExecutor _scenarioExecutor;
    private readonly ITelegramMessageManager _telegramMessageManager;
    private readonly IDataContext _dataContext;

    public RegistrationMoodleLoginEnteredState(
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
            await SendError(chat.ExtId, "Пожалуйста, введите свой login", ct);
            return;
        }

        var moodleAccount = await GetMoodleAccountAsync(update.Message.Text, ct);
        if (moodleAccount is null)
        {
            await SendError(chat.ExtId,
                "Я не узнаю этот login. Возможно, вы ввели его с ошибкой. Пожалуйста, повторите ввод", ct);
            return;
        }

        chat.CurrentScenarioArgs["STUDENT_FULL_NAME"] = moodleAccount.Student!.FullName;
        chat.CurrentScenarioArgs["MOODLE_LOGIN"] = moodleAccount.Username;
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