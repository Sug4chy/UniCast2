using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace UniCast.Application.TelegramBot.Utils;

public static class TelegramHelpers
{
    public static long GetChatId(Update update, Exception? exceptionToThrow = null)
        => update.Type switch
        {
            UpdateType.Message => update.Message!.Chat.Id,
            UpdateType.CallbackQuery => update.CallbackQuery!.Message!.Chat.Id,
            _ => throw exceptionToThrow ?? new ArgumentNullException(nameof(update))
        };

    public static int GetMessageId(Update update)
        => update.Type switch
        {
            UpdateType.Message => update.Message!.Id,
            _ => throw new ArgumentOutOfRangeException(nameof(update))
        };
}