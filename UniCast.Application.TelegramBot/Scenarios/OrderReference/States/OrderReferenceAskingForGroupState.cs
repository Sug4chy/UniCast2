using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using UniCast.Application.Abstractions.Persistence;
using UniCast.Application.Abstractions.Telegram;
using UniCast.Application.TelegramBot.Messages.Scenarios;
using UniCast.Application.TelegramBot.Utils;
using UniCast.Application.TelegramBot.Validation;
using UniCast.Domain.Telegram.Entities;

namespace UniCast.Application.TelegramBot.Scenarios.OrderReference.States;

public sealed class OrderReferenceAskingForGroupState : IOrderReferenceState
{
    private static readonly GroupNameValidator GroupNameValidator = new();

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

    private static string NormalizeGroupName(string groupName)
    {
        string[] groupNameParts = groupName.Split('-');
        string courseAndGroupNumber = groupNameParts[1];

        return groupNameParts[0].ToLower() switch
        {
            "при" => $"ПрИ-{courseAndGroupNumber}",
            "пи" => $"ПИ-{courseAndGroupNumber}",
            "би" => $"БИ-{courseAndGroupNumber}",
            _ => throw new ArgumentOutOfRangeException(nameof(groupName))
        };
    }

    public async Task OnStateChangedAsync(TelegramChat chat, Update update, CancellationToken ct = default)
    {
        var message = await _telegramMessageManager.SendMessageAsync(
            chat: chat,
            text: OrderReferenceScenarioMessages.EnterGroupName,
            ct: ct);
        chat.CurrentScenarioArgs[OrderReferenceScenarioArgsKeys.MessageToDeleteId] = message.ExtId.ToString();
        await _dataContext.SaveChangesAsync(ct);
    }

    public async Task HandleUserInputAsync(TelegramChat chat, Update update, CancellationToken ct = default)
    {
        await _scenarioExecutor.CancelAndThrowIfCancellationRequestedAsync(chat, update, ct);

        if (update is not { Type: UpdateType.Message, Message: not null, Message.Text: not null })
        {
            await _scenarioExecutor.HandleErrorAsync(
                chat: chat,
                messageId: TelegramHelpers.GetMessageId(update),
                errorText: OrderReferenceScenarioMessages.InvalidMessageFormat,
                ct: ct);
            return;
        }

        string groupName = update.Message.Text;
        var validationResult = await GroupNameValidator.ValidateAsync(groupName, ct);
        if (!validationResult.IsValid)
        {
            await _scenarioExecutor.HandleErrorAsync(
                chat: chat,
                messageId: TelegramHelpers.GetMessageId(update),
                errorText: string.Format(OrderReferenceScenarioMessages.InvalidGroupName,
                    string.Join('\n', validationResult.Errors.Select(x => $"- <b>{x.ErrorMessage}</b>"))),
                ct: ct);
            return;
        }

        chat.CurrentScenarioArgs[OrderReferenceScenarioArgsKeys.GroupName] = NormalizeGroupName(groupName);
        await _dataContext.SaveChangesAsync(ct);

        await _scenarioExecutor.ClearMessagesAsync(chat: chat, userMessageId: update.Message.Id, ct: ct);

        await _scenarioExecutor.ChangeStateAsync(
            chat: chat,
            newState: _scenarioExecutor.GetState((int)OrderReferenceState.AskingForReferencesCount),
            update: update,
            ct: ct);
    }
}