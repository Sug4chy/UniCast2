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

    public async Task<PrivateTelegramChat?> GetPrivateChatByExtIdAsync(long extId, CancellationToken ct = default)
    {
        const string query = $"""
                              SELECT id AS {nameof(PrivateTelegramChatDbModel.Id)},
                                     title AS {nameof(PrivateTelegramChatDbModel.Title)},
                                     ext_id AS {nameof(PrivateTelegramChatDbModel.ExtId)},
                                     type AS {nameof(PrivateTelegramChatDbModel.Type)},
                                     student_id AS {nameof(PrivateTelegramChatDbModel.StudentId)}
                              FROM telegram_chat
                              WHERE ext_id = @{nameof(extId)};
                              """;
        await using var connection = await _dbConnectionFactory.ConnectAsync(ct);

        return await connection.QuerySingleOrDefaultAsync<PrivateTelegramChat>(query, new { extId });
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

    public async Task AddPrivateAsync(PrivateTelegramChat chat, CancellationToken ct = default)
    {
        const string command = $"""
                                INSERT INTO telegram_chat(id, title, ext_id, type, student_id)
                                VALUES (
                                        @{nameof(PrivateTelegramChatDbModel.Id)},
                                        @{nameof(PrivateTelegramChatDbModel.Title)},
                                        @{nameof(PrivateTelegramChatDbModel.ExtId)},
                                        @{nameof(PrivateTelegramChatDbModel.Type)},
                                        @{nameof(PrivateTelegramChatDbModel.StudentId)}
                                );
                                """;
        await using var connection = await _dbConnectionFactory.ConnectAsync(ct);

        await connection.ExecuteAsync(command, chat.ToDbModel());
    }

    public Task UpdateScenarioAsync(PrivateTelegramChat chat, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}