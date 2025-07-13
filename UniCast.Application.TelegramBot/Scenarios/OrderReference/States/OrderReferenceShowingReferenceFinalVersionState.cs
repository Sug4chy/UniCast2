using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using UniCast.Application.Abstractions.Persistence;
using UniCast.Application.Abstractions.Telegram;
using UniCast.Application.TelegramBot.Messages.Scenarios;
using UniCast.Domain.Telegram.Entities;

namespace UniCast.Application.TelegramBot.Scenarios.OrderReference.States;

public sealed class OrderReferenceShowingReferenceFinalVersionState : IOrderReferenceState
{
    private const string Yes = "Да";
    private const string No = "Нет";

    private static readonly ReplyKeyboardMarkup YesOrNoKeyboard = new(
        (IEnumerable<KeyboardButton>) [new KeyboardButton(Yes), new KeyboardButton(No)]
    )
    {
        ResizeKeyboard = true
    };

    private readonly OrderReferenceScenarioExecutor _scenarioExecutor;
    private readonly ITelegramMessageManager _telegramMessageManager;
    private readonly IDataContext _dataContext;

    public OrderReferenceShowingReferenceFinalVersionState(
        OrderReferenceScenarioExecutor scenarioExecutor,
        IServiceProvider serviceProvider)
    {
        _scenarioExecutor = scenarioExecutor;
        _telegramMessageManager = serviceProvider.GetRequiredService<ITelegramMessageManager>();
        _dataContext = serviceProvider.GetRequiredService<IDataContext>();
    }

    private Task HandleYesAsync(TelegramChat chat, Update update, CancellationToken ct = default)
        => _scenarioExecutor.ChangeStateAsync(
            chat: chat,
            newState: _scenarioExecutor.GetState((int)OrderReferenceState.Completed),
            update: update,
            ct: ct);

    private async Task HandleNoAsync(TelegramChat chat, Update update, CancellationToken ct = default)
    {
        await _telegramMessageManager.SendMessageAsync(
            chatId: chat.ExtId,
            text: OrderReferenceScenarioMessages.OkLetsStartAgain,
            replyMarkup: new ReplyKeyboardRemove(),
            ct: ct);

        chat.CurrentScenarioArgs.Clear();
        await _dataContext.SaveChangesAsync(ct);

        await _scenarioExecutor.ChangeStateAsync(
            chat: chat,
            newState: _scenarioExecutor.GetState((int)OrderReferenceState.Started),
            update: update,
            ct: ct);
    }

    public async Task OnStateChangedAsync(TelegramChat chat, Update update, CancellationToken ct = default)
    {
        var student = await _dataContext.Students.FirstAsync(x => x.Id == chat.StudentId, ct);
        string studentFullName =
            $"{student.FullName.ToString()} {chat.CurrentScenarioArgs[OrderReferenceScenarioArgsKeys.Patronymic]}";

        string finalOrderVersionMessage = string.Format(OrderReferenceScenarioMessages.FinalOrderVersionMessageTemplate,
            studentFullName,
            chat.CurrentScenarioArgs[OrderReferenceScenarioArgsKeys.GroupName],
            chat.CurrentScenarioArgs[OrderReferenceScenarioArgsKeys.ReferencesCount],
            chat.CurrentScenarioArgs[OrderReferenceScenarioArgsKeys.OrderPurpose],
            chat.CurrentScenarioArgs[OrderReferenceScenarioArgsKeys.ObtainingMethod]
        );

        await _telegramMessageManager.SendMessageAsync(
            chatId: chat.ExtId,
            text: finalOrderVersionMessage,
            replyMarkup: YesOrNoKeyboard,
            ct: ct
        );
    }

    public async Task HandleUserInputAsync(TelegramChat chat, Update update, CancellationToken ct = default)
    {
        if (update is not { Type: UpdateType.Message, Message: not null, Message.Text: not null })
        {
            await _telegramMessageManager.SendMessageAsync(
                chatId: chat.ExtId,
                text: OrderReferenceScenarioMessages.InvalidMessageFormat,
                ct: ct);
            return;
        }

        switch (update.Message.Text)
        {
            case Yes:
                await HandleYesAsync(chat, update, ct);
                break;
            case No:
                await HandleNoAsync(chat, update, ct);
                break;
            default:
                await _telegramMessageManager.SendMessageAsync(
                    chatId: chat.ExtId,
                    text: OrderReferenceScenarioMessages.InvalidIsFinalVersionRightAnswer,
                    ct: ct);
                break;
        }
    }
}