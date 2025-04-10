using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using UniCast.Application.Abstractions.Moodle;
using UniCast.Application.Abstractions.Persistence;
using UniCast.Application.Abstractions.Telegram;
using UniCast.Domain.Moodle;
using UniCast.Domain.Telegram.Entities;

namespace UniCast.Application.TelegramBot.Scenarios.RefreshToken.States;

public sealed class RefreshTokenStartedState : IRefreshTokenState
{
    private static readonly ConcurrentDictionary<long, Update> UpdatesToDispatchByChats = new();
    private static readonly ConcurrentDictionary<long, Queue<int>> MessageIdsToDeleteByChats = new();

    private readonly RefreshTokenScenarioExecutor _scenarioExecutor;
    private readonly ITelegramMessageManager _telegramMessageManager;
    private readonly IDataContext _dataContext;
    private readonly IMoodleClient _moodleClient;

    public RefreshTokenStartedState(RefreshTokenScenarioExecutor scenarioExecutor, IServiceProvider serviceProvider)
    {
        _scenarioExecutor = scenarioExecutor;
        _telegramMessageManager = serviceProvider.GetRequiredService<ITelegramMessageManager>();
        _dataContext = serviceProvider.GetRequiredService<IDataContext>();
        _moodleClient = serviceProvider.GetRequiredService<IMoodleClient>();
    }

    public async Task OnStateChangedAsync(PrivateTelegramChat chat, Update update, CancellationToken ct = default)
    {
        UpdatesToDispatchByChats[chat.ExtId] = update;
        MessageIdsToDeleteByChats[chat.ExtId] = new Queue<int>();

        var message = await _telegramMessageManager.SendMessageAsync(
            chat: chat,
            text: "Время действия вашего пароля истекло. И раз уж мы его не сохраняем, то просим вас ввести его ещё" +
                  "раз, чтобы вы могли продолжать пользоваться ботом",
            ct: ct);

        MessageIdsToDeleteByChats[chat.ExtId].Enqueue(message.ExtId);
    }

    public async Task HandleUserInputAsync(PrivateTelegramChat chat, Update update, CancellationToken ct = default)
    {
        if (update.Message!.Text is null)
        {
            // TODO create messages for scenario
            await SendError(chat, "Введите пароль заново", ct);
            return;
        }

        var moodleAccount = await GetMoodleAccountAsync(chat.ExtId, ct);
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
            newState: _scenarioExecutor.GetState((int)RefreshTokenScenarioState.Completed),
            update: UpdatesToDispatchByChats[chat.ExtId],
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

    private Task<MoodleAccount> GetMoodleAccountAsync(long chatExtId, CancellationToken ct = default)
        => _dataContext.MoodleAccounts
            .SingleAsync(x => x.Student!.TelegramChat!.ExtId == chatExtId, ct);
}