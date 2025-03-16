using Telegram.Bot.Types.ReplyMarkups;

namespace UniCast.Application.Abstractions.Telegram;

public interface ITelegramMessageSender
{
    Task SendMessageAsync(
        long chatId, 
        string text, 
        InlineKeyboardMarkup? inlineKeyboard = null,
        CancellationToken ct = default);
}