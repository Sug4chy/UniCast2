using Telegram.Bot.Types.ReplyMarkups;
using UniCast.Domain.Telegram.Entities;

namespace UniCast.Application.Abstractions.Telegram;

public interface ITelegramMessageManager
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

    Task EditMessageAsync(
        long chatId,
        int messageId,
        string? newText = null,
        InlineKeyboardMarkup? newInlineKeyboard = null,
        CancellationToken ct = default
    );

    Task DeleteMessageAsync(
        long chatId,
        int messageId,
        CancellationToken ct = default);
}