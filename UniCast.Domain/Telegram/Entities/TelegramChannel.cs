using UniCast.Domain.Common.ValueObjects;
using UniCast.Domain.Students.Entities;
using UniCast.Domain.Telegram.ValueObjects.Enums;

namespace UniCast.Domain.Telegram.Entities;

public sealed class TelegramChannel : TelegramChat
{
    public override TelegramChatType Type { get; set; } = TelegramChatType.Channel;

    public IdOf<AcademicGroup> GroupId { get; init; }
    public AcademicGroup? AcademicGroup { get; init; }

    public static TelegramChannel Create(
        IdOf<TelegramChat> id,
        string title,
        long extId,
        AcademicGroup academicGroup)
    {
        ArgumentException.ThrowIfNullOrEmpty(title, nameof(title));
        ArgumentNullException.ThrowIfNull(academicGroup, nameof(academicGroup));

        return new TelegramChannel
        {
            Id = id,
            Title = title,
            ExtId = extId,
            GroupId = academicGroup.Id,
            AcademicGroup = academicGroup,
            Messages = []
        };
    }
}