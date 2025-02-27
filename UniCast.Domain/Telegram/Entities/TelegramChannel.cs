using CSharpFunctionalExtensions;
using UniCast.Domain.Common.ValueObjects;
using UniCast.Domain.Students.Entities;
using UniCast.Domain.Telegram.ValueObjects.Enums;

namespace UniCast.Domain.Telegram.Entities;

public sealed class TelegramChannel : TelegramChat
{
    public override TelegramChatType Type => TelegramChatType.Channel;
    public AcademicGroup AcademicGroup { get; }

    private TelegramChannel(
        IdOf<TelegramChat> id,
        string title,
        long extId,
        Maybe<List<TelegramMessage>> maybeMessages,
        AcademicGroup academicGroup) : base(id, title, extId, maybeMessages)
    {
        AcademicGroup = academicGroup;
    }

    public static Result<TelegramChannel> Create(
        string title,
        long extId,
        AcademicGroup academicGroup,
        Maybe<List<TelegramMessage>> maybeMessages = default)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return Result.Failure<TelegramChannel>("Название не должно быть пустым");
        }

        if (academicGroup is null)
        {
            return Result.Failure<TelegramChannel>("Группа должна быть указана");
        }

        return Result.Success(
            new TelegramChannel(
                id: IdOf<TelegramChat>.New(),
                title: title,
                extId: extId,
                maybeMessages: maybeMessages,
                academicGroup: academicGroup
            )
        );
    }
}