using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using UniCast.Application.Abstractions.Persistence;
using UniCast.Application.Abstractions.Telegram;
using UniCast.Application.Utlis;
using UniCast.Domain.Common.ValueObjects;
using UniCast.Domain.Telegram.Entities;
using UniCast.Domain.Telegram.ValueObjects.Enums;

namespace UniCast.Application.TelegramBot.Handlers.UserActions;

public sealed class UserReactedToMessageUpdateHandler : IUpdateHandler
{
    private readonly IDataContext _dataContext;
    private readonly ITelegramMessageManager _telegramMessageManager;
    private readonly ILogger<UserReactedToMessageUpdateHandler> _logger;

    public UserReactedToMessageUpdateHandler(
        IDataContext dataContext,
        ITelegramMessageManager telegramMessageManager,
        ILogger<UserReactedToMessageUpdateHandler> logger)
    {
        _dataContext = dataContext;
        _telegramMessageManager = telegramMessageManager;
        _logger = logger;
    }

    public ValueTask<bool> CanHandleAsync(Update update, CancellationToken ct = default)
        => ValueTask.FromResult(
            update.Type is UpdateType.CallbackQuery &&
            update.CallbackQuery!.Data is not null &&
            update.CallbackQuery.Data is TelegramConstants.ThumbUpReaction or TelegramConstants.ThumbDownReaction
        );

    public async Task HandleAsync(Update update, CancellationToken ct = default)
    {
        var message = await GetMessageWithReactionsByExtIdsPairAsync(
            update.CallbackQuery!.Message!.Chat.Id,
            update.CallbackQuery!.Message.Id,
            ct);
        if (message is null)
        {
            _logger.LogError("Message with extId {MessageExtId} in chat with extId {ChatExtId}",
                update.CallbackQuery!.Message.Id, update.CallbackQuery!.Message!.Chat.Id);
            return;
        }

        var reactionResult = CSharpFunctionalExtensions.Result.Try(
            () => TelegramMessageReaction.Create(
                id: IdOf<TelegramMessageReaction>.New(),
                reactorUsername: update.CallbackQuery.Message.Chat.Username!,
                reaction: update.CallbackQuery.Data!,
                message: message),
            e => e.Message);
        if (reactionResult.IsFailure)
        {
            _logger.LogError("{Error} occured while creating " + nameof(TelegramMessageReaction), reactionResult.Error);
            return;
        }

        message.Reactions.Add(reactionResult.Value);
        await _dataContext.SaveChangesAsync(ct);

        if (await IsChatPrivateAsync(update.CallbackQuery!.Message!.Chat.Id, ct))
        {
            await _telegramMessageManager.EditMessageAsync(
                chatId: update.CallbackQuery.Message.Chat.Id,
                messageId: update.CallbackQuery.Message.Id,
                newInlineKeyboard: InlineKeyboardMarkup.Empty(),
                ct: ct
            );
        }
    }

    private Task<TelegramMessage?> GetMessageWithReactionsByExtIdsPairAsync(
        long chatExtId,
        int messageExtId,
        CancellationToken ct = default)
        => _dataContext.TelegramMessages
            .Include(x => x.Reactions)
            .SingleOrDefaultAsync(x => x.ExtId == messageExtId &&
                                       x.Chat!.ExtId == chatExtId, ct);

    private Task<bool> IsChatPrivateAsync(long chatId, CancellationToken ct = default)
        => _dataContext.TelegramChats
            .AnyAsync(x => x.ExtId == chatId &&
                           x.Type == TelegramChatType.Private, ct);
}