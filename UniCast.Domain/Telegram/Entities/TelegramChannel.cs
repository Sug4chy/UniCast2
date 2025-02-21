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
}