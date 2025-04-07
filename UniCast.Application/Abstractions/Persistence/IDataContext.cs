using Microsoft.EntityFrameworkCore;
using UniCast.Domain.Messages.Entities;
using UniCast.Domain.Moodle;
using UniCast.Domain.Students.Entities;
using UniCast.Domain.Telegram.Entities;

namespace UniCast.Application.Abstractions.Persistence;

public interface IDataContext
{
    DbSet<AcademicGroup> AcademicGroups { get; init; }
    DbSet<MessageFromMethodist> MessageFromMethodists { get; init; }
    DbSet<Student> Students { get; init; }
    DbSet<TelegramChat> TelegramChats { get; init; }
    DbSet<TelegramMessage> TelegramMessages { get; init; }
    DbSet<TelegramMessageReaction> TelegramMessageReactions { get; init; }
    DbSet<MoodleAccount> MoodleAccounts { get; init; }
    DbSet<StudentsReply> StudentsReplies { get; init; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}