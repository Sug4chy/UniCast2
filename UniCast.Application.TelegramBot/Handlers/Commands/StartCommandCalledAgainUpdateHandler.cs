using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using UniCast.Application.Abstractions.Persistence;
using UniCast.Application.Abstractions.Telegram;

namespace UniCast.Application.TelegramBot.Handlers.Commands;

public sealed class StartCommandCalledAgainUpdateHandler : IUpdateHandler
{
    private readonly IDataContext _dataContext;
    private readonly ITelegramMessageManager _telegramMessageManager;

    public StartCommandCalledAgainUpdateHandler(
        IDataContext dataContext, 
        ITelegramMessageManager telegramMessageManager)
    {
        _dataContext = dataContext;
        _telegramMessageManager = telegramMessageManager;
    }

    public async ValueTask<bool> CanHandleAsync(Update update, CancellationToken ct = default)
        => update.Type is UpdateType.Message &&
           update.Message!.Text is not null &&
           update.Message.Text == "/start"
           && await _dataContext.TelegramChats
               .AnyAsync(x => x.ExtId == update.Message.Chat.Id &&
                              x.StudentId != null, ct);

    public Task HandleAsync(Update update, CancellationToken ct = default)
        => _telegramMessageManager.SendMessageAsync(
            chatId: update.Message!.Chat.Id,
            text: "Вы уже зарегистрированы.",
            ct: ct);
}