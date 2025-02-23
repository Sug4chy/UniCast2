using FastEndpoints;
using Telegram.Bot.Types;
using UniCast.Application.TelegramBot;

namespace UniCast.TelegramBot.Endpoints.V1.Updates;

public sealed class HandleUpdateEndpoint : Ep.Req<Update>.NoRes
{
    private readonly UpdateDispatcher _dispatcher;

    public HandleUpdateEndpoint(UpdateDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public override void Configure()
    {
        Post("/telegram-bot/v1/updates");
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(Update req, CancellationToken ct)
    {
        await _dispatcher.DispatchAsync(req, ct);
        await SendOkAsync(ct);
    }
}