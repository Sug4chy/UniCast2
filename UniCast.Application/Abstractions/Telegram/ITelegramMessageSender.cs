using Telegram.Bot.Types.ReplyMarkups;
using UniCast.Domain.Telegram.Entities;

namespace UniCast.Application.Abstractions.Telegram;

public interface ITelegramMessageSender
{
    Task SendMessageAsync(
        long chatId, 
        string text, 
        InlineKeyboardMarkup? inlineKeyboard = null,
        CancellationToken ct = default);

    Task<TelegramMessage> SendMessageAsync(
        TelegramChat chat,
        string text,
        InlineKeyboardMarkup? inlineKeyboard = null,
        CancellationToken ct = default);
}