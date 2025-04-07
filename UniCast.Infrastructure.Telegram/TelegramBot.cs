using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using UniCast.Application.Abstractions.Telegram;
using UniCast.Domain.Common.ValueObjects;
using UniCast.Domain.Telegram.Entities;

namespace UniCast.Infrastructure.Telegram;

public sealed class TelegramBot : ITelegramMessageManager
{
    private readonly ITelegramBotClient _telegramBotClient;

    public TelegramBot(ITelegramBotClient telegramBotClient)
    {
        _telegramBotClient = telegramBotClient;
    }

    public Task SendMessageAsync(
        long chatId,
        string text,
        InlineKeyboardMarkup? inlineKeyboard = null,
        CancellationToken ct = default)
        => _telegramBotClient.SendMessage(
            chatId: chatId,
            text: text,
            parseMode: ParseMode.Html,
            replyMarkup: inlineKeyboard,
            cancellationToken: ct);

    public async Task<TelegramMessage> SendMessageAsync(
        TelegramChat chat,
        string text,
        InlineKeyboardMarkup? inlineKeyboard = null,
        CancellationToken ct = default)
    {
        var message = await _telegramBotClient.SendMessage(
            chatId: chat.ExtId,
            text: text,
            parseMode: ParseMode.Html,
            replyMarkup: inlineKeyboard,
            cancellationToken: ct);

        return TelegramMessage.Create(
            id: IdOf<TelegramMessage>.New(),
            extId: message.Id,
            chat: chat,
            srcMessage: null!
        );
    }

    public async Task EditMessageAsync(
        long chatId,
        int messageId,
        string? newText = null,
        InlineKeyboardMarkup? newInlineKeyboard = null,
        CancellationToken ct = default)
    {
        if (newText is not null)
        {
            await _telegramBotClient.EditMessageText(
                chatId: chatId,
                messageId: messageId,
                text: newText,
                parseMode: ParseMode.Html,
                cancellationToken: ct);
        }

        if (newInlineKeyboard is not null)
        {
            await _telegramBotClient.EditMessageReplyMarkup(
                chatId: chatId,
                messageId: messageId,
                replyMarkup: newInlineKeyboard,
                cancellationToken: ct);
        }
    }

    public Task DeleteMessageAsync(long chatId, int messageId, CancellationToken ct = default)
        => _telegramBotClient.DeleteMessage(
            chatId: chatId,
            messageId: messageId,
            cancellationToken: ct);
}