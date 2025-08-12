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

public sealed class OrderReferenceAskingForReferenceOrderPurposeState : IOrderReferenceState
{
    private const string TransportCardPurpose = "Транспортная карта";
    private const string FaxDeductionPurpose = "Для работодателя";
    private const string SocialProtectionPurpose = "Соц. защита";
    private const string PensionFundPurpose = "Пенсионный фонд";
    private const string VisaExtensionPurpose = "Продление визы";
    private const string TaxPurpose = "Для налоговой";
    private const string OtherPurpose = "Другое";

    private static readonly string[] KeyboardButtonsTexts =
    [
        TransportCardPurpose,
        FaxDeductionPurpose,
        SocialProtectionPurpose,
        PensionFundPurpose,
        VisaExtensionPurpose,
        TaxPurpose,
        OtherPurpose
    ];

    private static readonly InlineKeyboardMarkup PurposesKeyboard = new(
        [
            [Button(SocialProtectionPurpose), Button(VisaExtensionPurpose)],
            [Button(TaxPurpose), Button(TransportCardPurpose)],
            [Button(FaxDeductionPurpose), Button(PensionFundPurpose)],
            [Button(OtherPurpose)]
        ]
    );

    private readonly OrderReferenceScenarioExecutor _scenarioExecutor;
    private readonly ITelegramMessageManager _telegramMessageManager;
    private readonly IDataContext _dataContext;

    public OrderReferenceAskingForReferenceOrderPurposeState(
        OrderReferenceScenarioExecutor scenarioExecutor,
        IServiceProvider serviceProvider)
    {
        _scenarioExecutor = scenarioExecutor;
        _telegramMessageManager = serviceProvider.GetRequiredService<ITelegramMessageManager>();
        _dataContext = serviceProvider.GetRequiredService<IDataContext>();
    }

    private static InlineKeyboardButton Button(string text) 
        => InlineKeyboardButton.WithCallbackData(text);

    private async Task HandleErrorAsync(
        TelegramChat chat,
        int messageId,
        string errorText,
        CancellationToken ct = default)
    {
        int botsPrevMessageId = int.Parse(chat.CurrentScenarioArgs[OrderReferenceScenarioArgsKeys.MessageToDeleteId]);
        await _telegramMessageManager.DeleteMessageAsync(chat.ExtId, botsPrevMessageId, ct);
        if (botsPrevMessageId != messageId)
        {
            await _telegramMessageManager.DeleteMessageAsync(chat.ExtId, messageId, ct);
        }

        var message = await _telegramMessageManager.SendMessageAsync(
            chat: chat,
            text: errorText,
            replyMarkup: PurposesKeyboard,
            ct: ct);
        chat.CurrentScenarioArgs[OrderReferenceScenarioArgsKeys.MessageToDeleteId] = message.ExtId.ToString();
        await _dataContext.SaveChangesAsync(ct);
    }

    private Task ClearStateMessagesAsync(TelegramChat chat, CancellationToken ct = default)
        => _telegramMessageManager.DeleteMessageAsync(
            chatId: chat.ExtId,
            messageId: int.Parse(chat.CurrentScenarioArgs[OrderReferenceScenarioArgsKeys.MessageToDeleteId]),
            ct: ct);

    public async Task OnStateChangedAsync(TelegramChat chat, Update update, CancellationToken ct = default)
    {
        var message = await _telegramMessageManager.SendMessageAsync(
            chat: chat,
            text: OrderReferenceScenarioMessages.ChooseReferenceOrderPurpose,
            replyMarkup: PurposesKeyboard,
            ct: ct);
        chat.CurrentScenarioArgs[OrderReferenceScenarioArgsKeys.MessageToDeleteId] = message.ExtId.ToString();
        await _dataContext.SaveChangesAsync(ct);
    }

    public async Task HandleUserInputAsync(TelegramChat chat, Update update, CancellationToken ct = default)
    {
        if (update is not { Type: UpdateType.CallbackQuery, CallbackQuery.Data: not null })
        {
            await HandleErrorAsync(
                chat: chat,
                messageId: TelegramHelpers.GetMessageId(update),
                errorText: OrderReferenceScenarioMessages.InvalidMessageFormat,
                ct: ct);
            return;
        }

        string orderPurpose = update.CallbackQuery.Data;
        if (!KeyboardButtonsTexts.Contains(orderPurpose))
        {
            await HandleErrorAsync(
                chat: chat,
                messageId: update.CallbackQuery.Message!.Id,
                errorText: OrderReferenceScenarioMessages.InvalidPurpose,
                ct: ct);
            return;
        }

        if (orderPurpose == OtherPurpose)
        {
            await ClearStateMessagesAsync(chat: chat, ct: ct);
            await _scenarioExecutor.ChangeStateAsync(
                chat: chat,
                newState: _scenarioExecutor.GetState((int)OrderReferenceState.OtherOrderPurposeSelected),
                update: update,
                ct: ct);
            return;
        }

        chat.CurrentScenarioArgs[OrderReferenceScenarioArgsKeys.OrderPurpose] = orderPurpose;
        await _dataContext.SaveChangesAsync(ct);

        await ClearStateMessagesAsync(chat: chat, ct: ct);

        await _scenarioExecutor.ChangeStateAsync(
            chat: chat,
            newState: _scenarioExecutor.GetState((int)OrderReferenceState.AskingForReferenceObtainingMethod),
            update: update,
            ct: ct);
    }
}