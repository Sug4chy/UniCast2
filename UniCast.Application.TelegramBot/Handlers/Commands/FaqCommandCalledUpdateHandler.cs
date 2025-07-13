using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using UniCast.Application.Abstractions.Telegram;
using UniCast.Application.TelegramBot.Messages.Commands;

namespace UniCast.Application.TelegramBot.Handlers.Commands;

public sealed class FaqCommandCalledUpdateHandler : IUpdateHandler
{
    private readonly ITelegramMessageManager _telegramMessageManager;

    public FaqCommandCalledUpdateHandler(ITelegramMessageManager telegramMessageManager)
    {
        _telegramMessageManager = telegramMessageManager;
    }

    public ValueTask<bool> CanHandleAsync(Update update, CancellationToken ct = default)
        => ValueTask.FromResult(
            update.Type is UpdateType.Message &&
            update.Message!.Text is not null &&
            update.Message.Text.StartsWith("/faq"));

    public Task HandleAsync(Update update, CancellationToken ct = default)
        => _telegramMessageManager.SendMessageAsync(
            chatId: update.Message!.Chat.Id,
            text: FaqCommandMessages.NewResponse,
            replyMarkup: new InlineKeyboardMarkup(
                InlineKeyboardButton.WithUrl("Не нашли ответ на интересующий вопрос?", "https://iit.csu.ru/")),
            ct: ct);
}