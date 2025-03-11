using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using UniCast.Application.Abstractions.Repositories;
using UniCast.Application.TelegramBot.Utlis;
using UniCast.Domain.Common.ValueObjects;
using UniCast.Domain.Telegram.Entities;

namespace UniCast.Application.TelegramBot.Handlers.UserActions;

public sealed class UserReactedToMessageUpdateHandler : IUpdateHandler
{
    private readonly ITelegramMessageRepository _telegramMessageRepository;
    private readonly ILogger<UserReactedToMessageUpdateHandler> _logger;

    public UserReactedToMessageUpdateHandler(
        ITelegramMessageRepository telegramMessageRepository,
        ILogger<UserReactedToMessageUpdateHandler> logger)
    {
        _telegramMessageRepository = telegramMessageRepository;
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
        var message = await _telegramMessageRepository.GetMessageWithReactionsByExtIdsPairAsync(
            update.CallbackQuery!.Message!.Chat.Id,
            update.CallbackQuery!.Message.Id,
            ct);
        if (message is null)
        {
            _logger.LogError("Message with extId {MessageExtId} in chat with extId {ChatExtId}",
                update.CallbackQuery!.Message.Id, update.CallbackQuery!.Message!.Chat.Id);
            return;
        }

        var reactionResult = TelegramMessageReaction.Create(
            id: IdOf<TelegramMessageReaction>.New(),
            reactorUsername: update.CallbackQuery.Message.Chat.Username!,
            reaction: update.CallbackQuery.Data!,
            message: message);
        if (reactionResult.IsFailure)
        {
            _logger.LogError("{Error} occured while creating " + nameof(TelegramMessageReaction), reactionResult.Error);
        }

        message.AddReaction(reactionResult.Value);
        await _telegramMessageRepository.AddMessageReactionsAsync(message, ct);
    }
}