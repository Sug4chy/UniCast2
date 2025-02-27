namespace UniCast.Application.Abstractions.Telegram;

public interface ITelegramMessageSender
{
    Task SendMessageAsync(long chatId, string text, CancellationToken ct = default);
}