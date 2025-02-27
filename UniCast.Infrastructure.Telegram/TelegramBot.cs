using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using UniCast.Application.Abstractions.Telegram;

namespace UniCast.Infrastructure.Telegram;

public sealed class TelegramBot : ITelegramMessageSender
{
    private readonly ITelegramBotClient _telegramBotClient;

    public TelegramBot(ITelegramBotClient telegramBotClient)
    {
        _telegramBotClient = telegramBotClient;
    }

    public Task SendMessageAsync(long chatId, string text, CancellationToken ct = default)
        => _telegramBotClient.SendMessage(
            chatId: chatId,
            text: text,
            parseMode: ParseMode.Html,
            cancellationToken: ct);
}