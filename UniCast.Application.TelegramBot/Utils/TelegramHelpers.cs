using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace UniCast.Application.TelegramBot.Utlis;

public static class TelegramHelpers
{
    public static long GetChatId(Update update, Exception? exceptionToThrow = null)
        => update.Type switch
        {
            UpdateType.Message => update.Message!.Chat.Id,
            UpdateType.CallbackQuery => update.CallbackQuery!.Message!.Chat.Id,
            _ => throw exceptionToThrow ?? new ArgumentNullException(nameof(update))
        };
}