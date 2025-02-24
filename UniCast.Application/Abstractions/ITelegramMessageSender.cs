namespace UniCast.Application.Abstractions;

public interface ITelegramMessageSender
{
    Task SendMessageAsync(long chatId, string text, CancellationToken ct = default);
}