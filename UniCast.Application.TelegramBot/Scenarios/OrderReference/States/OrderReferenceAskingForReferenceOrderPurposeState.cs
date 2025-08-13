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
            [InlineButton(SocialProtectionPurpose), InlineButton(VisaExtensionPurpose)],
            [InlineButton(TaxPurpose), InlineButton(TransportCardPurpose)],
            [InlineButton(FaxDeductionPurpose), InlineButton(PensionFundPurpose)],
            [InlineButton(OtherPurpose)]
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

    private static InlineKeyboardButton InlineButton(string text) 
        => InlineKeyboardButton.WithCallbackData(text);

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
        await _scenarioExecutor.CancelAndThrowIfCancellationRequestedAsync(chat, update, ct);

        if (update is not { Type: UpdateType.CallbackQuery, CallbackQuery.Data: not null })
        {
            await _scenarioExecutor.HandleErrorAsync(
                chat: chat,
                messageId: TelegramHelpers.GetMessageId(update),
                errorText: OrderReferenceScenarioMessages.InvalidMessageFormat,
                ct: ct);
            return;
        }

        string orderPurpose = update.CallbackQuery.Data;
        if (!KeyboardButtonsTexts.Contains(orderPurpose))
        {
            await _scenarioExecutor.HandleErrorAsync(
                chat: chat,
                messageId: update.CallbackQuery.Message!.Id,
                errorText: OrderReferenceScenarioMessages.InvalidPurpose,
                ct: ct);
            return;
        }

        if (orderPurpose == OtherPurpose)
        {
            await _scenarioExecutor.ClearMessagesAsync(chat: chat, ct: ct);
            await _scenarioExecutor.ChangeStateAsync(
                chat: chat,
                newState: _scenarioExecutor.GetState((int)OrderReferenceState.OtherOrderPurposeSelected),
                update: update,
                ct: ct);
            return;
        }

        chat.CurrentScenarioArgs[OrderReferenceScenarioArgsKeys.OrderPurpose] = orderPurpose;
        await _dataContext.SaveChangesAsync(ct);

        await _scenarioExecutor.ClearMessagesAsync(chat: chat, ct: ct);

        await _scenarioExecutor.ChangeStateAsync(
            chat: chat,
            newState: _scenarioExecutor.GetState((int)OrderReferenceState.AskingForReferenceObtainingMethod),
            update: update,
            ct: ct);
    }
}