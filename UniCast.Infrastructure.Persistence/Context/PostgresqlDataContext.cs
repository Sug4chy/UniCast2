using Microsoft.EntityFrameworkCore;
using UniCast.Application.Abstractions.Persistence;
using UniCast.Domain.Messages.Entities;
using UniCast.Domain.Students.Entities;
using UniCast.Domain.Telegram.Entities;

namespace UniCast.Infrastructure.Persistence.Context;

public sealed class PostgresqlDataContext(
    DbContextOptions<PostgresqlDataContext> options
) : DbContext(options), IDataContext
{
    public DbSet<AcademicGroup> AcademicGroups { get; init; }
    public DbSet<MessageFromMethodist> MessageFromMethodists { get; init; }
    public DbSet<Student> Students { get; init; }
    public DbSet<TelegramChat> TelegramChats { get; init; }
    public DbSet<TelegramMessage> TelegramMessages { get; init; }
    public DbSet<TelegramMessageReaction> TelegramMessageReactions { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        base.OnModelCreating(modelBuilder);
    }
}