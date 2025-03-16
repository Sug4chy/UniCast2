using UniCast.Domain.Telegram.Entities;

namespace UniCast.Application.Abstractions.Repositories;

public interface ITelegramChatRepository
{
    Task<PrivateTelegramChat?> GetPrivateChatByExtIdAsync(long extId, CancellationToken ct = default);
    Task AddChannelAsync(TelegramChannel chat, CancellationToken ct = default);
    Task AddPrivateAsync(PrivateTelegramChat chat, CancellationToken ct = default);
    Task UpdateScenarioAsync(PrivateTelegramChat chat, CancellationToken ct = default);
}