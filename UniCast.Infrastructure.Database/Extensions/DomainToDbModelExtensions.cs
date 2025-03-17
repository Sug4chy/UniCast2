using System.Text.Json;
using UniCast.Domain.Students.Entities;
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

    public static PrivateTelegramChatDbModel ToDbModel(this PrivateTelegramChat telegramChat)
        => new(
            Id: telegramChat.Id,
            Title: telegramChat.Title,
            ExtId: telegramChat.ExtId,
            Type: (byte)telegramChat.Type,
            StudentId: telegramChat.Student.HasValue
                ? telegramChat.Student.Value.Id
                : null,
            CurrentScenario: telegramChat.CurrentScenario.HasValue
                ? (int)telegramChat.CurrentScenario.Value
                : null,
            CurrentState: telegramChat.CurrentState.HasValue
                ? telegramChat.CurrentState.Value
                : null,
            CurrentScenarioArgs: telegramChat.CurrentScenarioArgs.Count == 0
                ? null
                : JsonSerializer.Serialize(telegramChat.CurrentScenarioArgs)
        );

    public static StudentDbModel ToDbModel(this Student student)
        => new(
            Id: student.Id,
            FullName: student.FullName,
            GroupId: student.Group.Id
        );
}