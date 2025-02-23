using Telegram.Bot.Types;

namespace UniCast.Application.TelegramBot.Handlers;

public interface IUpdateHandler
{
    ValueTask<bool> CanHandleAsync(Update update, CancellationToken ct = default);
    Task HandleAsync(Update update, CancellationToken ct = default);
}