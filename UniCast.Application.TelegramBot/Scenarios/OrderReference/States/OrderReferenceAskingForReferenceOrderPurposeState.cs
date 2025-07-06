using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using UniCast.Application.Abstractions.Telegram;
using UniCast.Application.TelegramBot.Messages.Scenarios;
using UniCast.Domain.Telegram.Entities;

namespace UniCast.Application.TelegramBot.Scenarios.OrderReference.States;

public sealed class OrderReferenceAskingForReferenceOrderPurposeState : IOrderReferenceState
{
    private static readonly IEnumerable<string> KeyboardButtonsTexts =
    [
        OrderReferenceScenarioMessages.TransportCardReferenceOrderPurpose,
        OrderReferenceScenarioMessages.ParentsFaxDeductionReferenceOrderPurpose,
        OrderReferenceScenarioMessages.OtherReferenceOrderPurpose
    ];
    private static readonly ReplyKeyboardMarkup PurposesKeyboard = new(
        KeyboardButtonsTexts.Select(x => new KeyboardButton(x))
    );

    private readonly OrderReferenceScenarioExecutor _scenarioExecutor;
    private readonly ITelegramMessageManager _telegramMessageManager;

    public OrderReferenceAskingForReferenceOrderPurposeState(
        OrderReferenceScenarioExecutor scenarioExecutor,
        IServiceProvider serviceProvider)
    {
        _scenarioExecutor = scenarioExecutor;
        _telegramMessageManager = serviceProvider.GetRequiredService<ITelegramMessageManager>();
    }

    public Task OnStateChangedAsync(TelegramChat chat, Update update, CancellationToken ct = default)
        => _telegramMessageManager.SendMessageAsync(
            chatId: chat.ExtId,
            text: OrderReferenceScenarioMessages.EnterReferenceOrderPurpose,
            replyMarkup: PurposesKeyboard,
            ct: ct);

    public Task HandleUserInputAsync(TelegramChat chat, Update update, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}