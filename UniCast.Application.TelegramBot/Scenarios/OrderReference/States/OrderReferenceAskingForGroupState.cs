using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using UniCast.Application.Abstractions.Persistence;
using UniCast.Application.Abstractions.Telegram;
using UniCast.Application.TelegramBot.Messages.Scenarios;
using UniCast.Application.TelegramBot.Utils;
using UniCast.Domain.Telegram.Entities;

namespace UniCast.Application.TelegramBot.Scenarios.OrderReference.States;

public sealed partial class OrderReferenceAskingForGroupState : IOrderReferenceState
{
    [GeneratedRegex("^(ПрИ|ПИ|БИ)-[1-4]0[1-9]")]
    private static partial Regex GroupNameRegex();

    private readonly OrderReferenceScenarioExecutor _scenarioExecutor;
    private readonly ITelegramMessageManager _telegramMessageManager;
    private readonly IDataContext _dataContext;

    public OrderReferenceAskingForGroupState(
        OrderReferenceScenarioExecutor scenarioExecutor,
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
            text: OrderReferenceScenarioMessages.EnterGroupName,
            replyMarkup: new ReplyKeyboardRemove(),
            ct: ct);
        chat.CurrentScenarioArgs[OrderReferenceScenarioArgsKeys.MessageToDeleteId] = message.ExtId.ToString();
        await _dataContext.SaveChangesAsync(ct);
    }

    public async Task HandleUserInputAsync(TelegramChat chat, Update update, CancellationToken ct = default)
    {
        if (update is not { Type: UpdateType.Message, Message: not null, Message.Text: not null })
        {
            await HandleErrorAsync(
                chat: chat,
                messageId: TelegramHelpers.GetMessageId(update),
                errorText: OrderReferenceScenarioMessages.InvalidMessageFormat,
                ct: ct);
            return;
        }

        if (!GroupNameRegex().IsMatch(update.Message.Text))
        {
            await HandleErrorAsync(
                chat: chat,
                messageId: TelegramHelpers.GetMessageId(update),
                errorText: OrderReferenceScenarioMessages.InvalidGroupName,
                ct: ct);
            return;
        }

        chat.CurrentScenarioArgs[OrderReferenceScenarioArgsKeys.GroupName] = update.Message.Text;
        await _dataContext.SaveChangesAsync(ct);

        await _telegramMessageManager.DeleteMessageAsync(
            chatId: chat.ExtId,
            messageId: int.Parse(chat.CurrentScenarioArgs[OrderReferenceScenarioArgsKeys.MessageToDeleteId]),
            ct: ct);
        await _telegramMessageManager.DeleteMessageAsync(chat.ExtId, update.Message.MessageId, ct);

        await _scenarioExecutor.ChangeStateAsync(
            chat: chat,
            newState: _scenarioExecutor.GetState((int)OrderReferenceState.AskingForReferencesCount),
            update: update,
            ct: ct);
    }

    private async Task HandleErrorAsync(
        TelegramChat chat,
        int messageId,
        string errorText,
        CancellationToken ct = default)
    {
        int botsPrevMessageId = int.Parse(chat.CurrentScenarioArgs[OrderReferenceScenarioArgsKeys.MessageToDeleteId]);
        await _telegramMessageManager.DeleteMessageAsync(chat.ExtId, botsPrevMessageId, ct);
        await _telegramMessageManager.DeleteMessageAsync(chat.ExtId, messageId, ct);

        var message = await _telegramMessageManager.SendMessageAsync(chat: chat, text: errorText, ct: ct);
        chat.CurrentScenarioArgs[OrderReferenceScenarioArgsKeys.MessageToDeleteId] = message.ExtId.ToString();
        await _dataContext.SaveChangesAsync(ct);
    }
}