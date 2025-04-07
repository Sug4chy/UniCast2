using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using UniCast.Application.Abstractions.Moodle;
using UniCast.Application.Abstractions.Persistence;
using UniCast.Application.Abstractions.Telegram;
using UniCast.Domain.Moodle;
using UniCast.Domain.Telegram.Entities;

namespace UniCast.Application.TelegramBot.Scenarios.Registration.States;

public sealed class RegistrationWaitingForMoodlePasswordEnteredState : IRegistrationState
{
    private static readonly ConcurrentDictionary<long, Queue<int>> MessageIdsToDeleteByChats = new();

    private readonly RegistrationScenarioExecutor _scenarioExecutor;
    private readonly ITelegramMessageManager _telegramMessageManager;
    private readonly IMoodleClient _moodleClient;
    private readonly IDataContext _dataContext;

    public RegistrationWaitingForMoodlePasswordEnteredState(
        RegistrationScenarioExecutor scenarioExecutor,
        IServiceProvider serviceProvider)
    {
        _scenarioExecutor = scenarioExecutor;
        _telegramMessageManager = serviceProvider.GetRequiredService<ITelegramMessageManager>();
        _dataContext = serviceProvider.GetRequiredService<IDataContext>();
        _moodleClient = serviceProvider.GetRequiredService<IMoodleClient>();
    }

    public async Task OnStateChangedAsync(PrivateTelegramChat chat, Update update, CancellationToken ct = default)
    {
        var message = await _telegramMessageManager.SendMessageAsync(
            chat: chat,
            text: "Введите свой пароль от Moodle. Он не будет сохранён, и нужен для того, чтобы методисты видели " +
                  "ваши сообщения именно от вашего имени",
            ct: ct);

        MessageIdsToDeleteByChats[chat.ExtId] = new Queue<int>();
        MessageIdsToDeleteByChats[chat.ExtId].Enqueue(message.ExtId);

        await _dataContext.SaveChangesAsync(ct);
    }

    public async Task HandleUserInputAsync(PrivateTelegramChat chat, Update update, CancellationToken ct = default)
    {
        if (update.Message!.Text is null)
        {
            await SendError(chat, "Пожалуйста, введите свой пароль", ct);
            return;
        }

        var moodleAccount = await GetMoodleAccountAsync(chat.CurrentScenarioArgs["MOODLE_LOGIN"], ct);
        string password = update.Message!.Text!;

        MessageIdsToDeleteByChats[chat.ExtId].Enqueue(update.Message.Id);
        while (MessageIdsToDeleteByChats[chat.ExtId].TryDequeue(out int messageId))
        {
            await _telegramMessageManager.DeleteMessageAsync(
                chatId: chat.ExtId,
                messageId: messageId,
                ct: ct);
        }

        var tokenResult = await _moodleClient.LoginAsync(moodleAccount.Username, password, ct);
        if (tokenResult.IsFailure)
        {
            await SendError(chat, tokenResult.Error!, ct);
            return;
        }

        moodleAccount.CurrentToken = tokenResult.Value;

        await _scenarioExecutor.ChangeStateAsync(
            chat: chat,
            newState: _scenarioExecutor.GetState((int)RegistrationScenarioState.Completed),
            update: update,
            ct: ct);
    }

    private async Task SendError(PrivateTelegramChat chat, string errorText, CancellationToken ct = default)
    {
        var message = await _telegramMessageManager.SendMessageAsync(
            chat: chat,
            text: errorText,
            ct: ct);

        MessageIdsToDeleteByChats[chat.ExtId].Enqueue(message.ExtId);
    }

    private Task<MoodleAccount> GetMoodleAccountAsync(string username, CancellationToken ct = default)
        => _dataContext.MoodleAccounts
            .SingleAsync(x => x.Username == username, ct);
}