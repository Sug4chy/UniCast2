using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using UniCast.Application.TelegramBot.Exceptions;

namespace UniCast.Application.TelegramBot.Utils;

public static class TelegramHelpers
{
    public static long GetChatId(Update update)
        => update.Type switch
        {
            UpdateType.Message => update.Message!.Chat.Id,
            UpdateType.CallbackQuery => update.CallbackQuery!.Message!.Chat.Id,
            _ => throw new UnknownUpdateTypeException()
        };

    public static int GetMessageId(Update update)
        => update.Type switch
        {
            UpdateType.Message => update.Message!.Id,
            UpdateType.CallbackQuery => update.CallbackQuery!.Message!.Id,
            _ => throw new UnknownUpdateTypeException()
        };

    public static bool TryGetUserInput(Update update, out string userInput)
    {
        userInput = (update.Type switch
        {
            UpdateType.Message => update.Message!.Text,
            UpdateType.CallbackQuery => update.CallbackQuery!.Data,
            _ => throw new UnknownUpdateTypeException()
        })!;

        return userInput is not null;
    }
}