using Dapper;
using UniCast.Application.Abstractions.Repositories;
using UniCast.Domain.Telegram.Entities;
using UniCast.Infrastructure.Database.Attributes;
using UniCast.Infrastructure.Database.ConnectionFactory;
using UniCast.Infrastructure.Database.Extensions;
using UniCast.Infrastructure.Database.Models;

namespace UniCast.Infrastructure.Database.Repositories;

[Repository]
public sealed class TelegramChatRepository : ITelegramChatRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public TelegramChatRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task AddChannelAsync(TelegramChannel chat, CancellationToken ct = default)
    {
        const string command = $"""
                                INSERT INTO telegram_chat(id, title, ext_id, type, group_id)
                                VALUES (
                                        @{nameof(TelegramChannelDbModel.Id)},
                                        @{nameof(TelegramChannelDbModel.Title)},
                                        @{nameof(TelegramChannelDbModel.ExtId)},
                                        @{nameof(TelegramChannelDbModel.Type)},
                                        @{nameof(TelegramChannelDbModel.GroupId)}
                                );
                                """;
        await using var connection = await _dbConnectionFactory.ConnectAsync(ct);
        await connection.ExecuteAsync(command, chat.ToDbModel());
    }
}