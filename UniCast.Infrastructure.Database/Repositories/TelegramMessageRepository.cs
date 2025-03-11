using Dapper;
using UniCast.Application.Abstractions.Repositories;
using UniCast.Domain.Telegram.Entities;
using UniCast.Infrastructure.Database.Attributes;
using UniCast.Infrastructure.Database.ConnectionFactory;
using UniCast.Infrastructure.Database.Extensions;
using UniCast.Infrastructure.Database.Models;

namespace UniCast.Infrastructure.Database.Repositories;

[Repository]
public sealed class TelegramMessageRepository : ITelegramMessageRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public TelegramMessageRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<TelegramMessage?> GetMessageWithReactionsByExtIdsPairAsync(
        long chatExtId,
        int messageExtId,
        CancellationToken ct = default)
    {
        const string messageQuery = $"""
                                     SELECT tm.id AS {nameof(TelegramMessageDbModel.Id)},
                                            tm.ext_id AS {nameof(TelegramMessageDbModel.ExtId)},
                                            tm.chat_id AS {nameof(TelegramMessageDbModel.ChatId)},
                                            tm.src_message_id AS {nameof(TelegramMessageDbModel.SrcMessageId)}
                                     FROM telegram_message tm
                                        LEFT JOIN telegram_chat tc on tc.id = tm.chat_id
                                     WHERE tc.ext_id = @{nameof(chatExtId)} 
                                       AND tm.ext_id = @{nameof(messageExtId)};
                                     """;
        await using var connection = await _dbConnectionFactory.ConnectAsync(ct);
        var message = await connection.QuerySingleOrDefaultAsync<TelegramMessageDbModel?>(
            messageQuery, new { chatExtId, messageExtId });

        if (message is null)
        {
            return null;
        }

        const string reactionsQuery = $"""
                                       SELECT id AS {nameof(TelegramMessageReactionDbModel.Id)},
                                              reactor_username AS {nameof(TelegramMessageReactionDbModel.ReactorUsername)},
                                              reaction AS {nameof(TelegramMessageReactionDbModel.Reaction)},
                                              message_id AS {nameof(TelegramMessageReactionDbModel.MessageId)}
                                       FROM telegram_message_reaction
                                       WHERE message_id = @{nameof(TelegramMessageDbModel.Id)};
                                       """;
        ct.ThrowIfCancellationRequested();

        var reactions = await connection.QueryAsync<TelegramMessageReactionDbModel>(
            reactionsQuery, message);

        var domainMessage = message.ToDomain();
        domainMessage.AddReactions(reactions.Select(r => r.ToDomain(domainMessage)).ToList());

        return domainMessage;
    }

    public async Task AddMessageReactionsAsync(TelegramMessage message, CancellationToken ct = default)
    {
        const string query = $"""
                              INSERT INTO telegram_message_reaction(id, reactor_username, reaction, message_id)
                              VALUES (@{nameof(TelegramMessageReactionDbModel.Id)},
                                      @{nameof(TelegramMessageReactionDbModel.ReactorUsername)},
                                      @{nameof(TelegramMessageReactionDbModel.Reaction)},
                                      @{nameof(TelegramMessageReactionDbModel.MessageId)})
                              ON CONFLICT DO NOTHING;
                              """;
        await using var connection = await _dbConnectionFactory.ConnectAsync(ct);

        await connection.ExecuteAsync(query, message.Reactions.Select(x => x.ToDbModel()).ToList());
    }
}