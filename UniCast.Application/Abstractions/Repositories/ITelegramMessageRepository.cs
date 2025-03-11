using UniCast.Domain.Telegram.Entities;

namespace UniCast.Application.Abstractions.Repositories;

public interface ITelegramMessageRepository
{
    Task<TelegramMessage?> GetMessageWithReactionsByExtIdsPairAsync(
        long chatExtId,
        int messageExtId,
        CancellationToken ct = default);

    Task AddMessageReactionsAsync(TelegramMessage message, CancellationToken ct = default);
}