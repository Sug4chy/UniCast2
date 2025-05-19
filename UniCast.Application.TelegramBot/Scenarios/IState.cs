using Telegram.Bot.Types;
using UniCast.Domain.Telegram.Entities;

namespace UniCast.Application.TelegramBot.Scenarios;

public interface IState
{
    Task OnStateChangedAsync(TelegramChat chat, Update update, CancellationToken ct = default);
    Task HandleUserInputAsync(TelegramChat chat, Update update, CancellationToken ct = default);
}