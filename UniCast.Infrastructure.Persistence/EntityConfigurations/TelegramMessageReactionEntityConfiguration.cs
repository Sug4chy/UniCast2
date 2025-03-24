using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniCast.Domain.Telegram.Entities;
using UniCast.Infrastructure.Persistence.Extensions;

namespace UniCast.Infrastructure.Persistence.EntityConfigurations;

public sealed class TelegramMessageReactionEntityConfiguration : IEntityTypeConfiguration<TelegramMessageReaction>
{
    public void Configure(EntityTypeBuilder<TelegramMessageReaction> builder)
    {
        builder.ToTable(nameof(TelegramMessageReaction).ToSnakeCase());

        builder.HasId();

        builder.Property(x => x.ReactorUsername)
            .IsRequired()
            .HasColumnName(nameof(TelegramMessageReaction.ReactorUsername).ToSnakeCase());

        builder.Property(x => x.Reaction)
            .IsRequired()
            .HasColumnName(nameof(TelegramMessageReaction.Reaction).ToSnakeCase());

        builder.Property(x => x.MessageId)
            .IsRequired()
            .HasDefaultIdConversion()
            .HasColumnName(nameof(TelegramMessageReaction.MessageId).ToSnakeCase());
        builder.HasOne(x => x.Message)
            .WithMany(x => x.Reactions)
            .HasForeignKey(x => x.MessageId);
    }
}