using UniCast.Domain.Telegram.Entities;

namespace UniCast.Application.Abstractions.Repositories;

public interface ITelegramChatRepository
{
    Task AddChannelAsync(TelegramChannel chat, CancellationToken ct = default);
}