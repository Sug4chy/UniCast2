using UniCast.Domain.Telegram.Entities;
using UniCast.Infrastructure.Database.Models;

namespace UniCast.Infrastructure.Database.Extensions;

public static class DomainToDbModelExtensions
{
    public static TelegramChannelDbModel ToDbModel(this TelegramChannel telegramChannel)
        => new(
            Id: telegramChannel.Id,
            Title: telegramChannel.Title,
            ExtId: telegramChannel.ExtId,
            Type: (byte)telegramChannel.Type,
            GroupId: telegramChannel.AcademicGroup.Id
        );

    public static TelegramMessageReactionDbModel ToDbModel(this TelegramMessageReaction reaction)
        => new(
            Id: reaction.Id,
            ReactorUsername: reaction.ReactorUsername,
            Reaction: reaction.Reaction,
            MessageId: reaction.Message.Id
        );
}