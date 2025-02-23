using Telegram.Bot.Types;
using UniCast.Application.TelegramBot.Handlers;

namespace UniCast.Application.TelegramBot;

public sealed class UpdateDispatcher
{
    private readonly IEnumerable<IUpdateHandler> _handlers;

    public UpdateDispatcher(IEnumerable<IUpdateHandler> handlers)
    {
        _handlers = handlers;
    }

    public async Task DispatchAsync(Update update, CancellationToken ct = default)
    {
        foreach (var handler in _handlers)
        {
            if (!await handler.CanHandleAsync(update, ct))
            {
                continue;
            }

            await handler.HandleAsync(update, ct);
            break;
        }
    }
}