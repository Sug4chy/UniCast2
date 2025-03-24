using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniCast.Domain.Messages.Entities;
using UniCast.Domain.Telegram.Entities;
using UniCast.Infrastructure.Persistence.Extensions;

namespace UniCast.Infrastructure.Persistence.EntityConfigurations;

public sealed class TelegramMessageEntityConfiguration : IEntityTypeConfiguration<TelegramMessage>
{
    public void Configure(EntityTypeBuilder<TelegramMessage> builder)
    {
        builder.ToTable(nameof(TelegramMessage).ToSnakeCase());

        builder.HasId();

        builder.Property(x => x.ExtId)
            .IsRequired()
            .HasColumnName(nameof(TelegramMessage.ExtId).ToSnakeCase());

        builder.Property(x => x.ChatId)
            .IsRequired()
            .HasDefaultIdConversion()
            .HasColumnName(nameof(TelegramMessage.ChatId).ToSnakeCase());
        builder.HasOne(x => x.Chat)
            .WithMany(x => x.Messages)
            .HasForeignKey(x => x.ChatId);

        builder.Property(x => x.SrcMessageId)
            .IsRequired()
            .HasDefaultIdConversion()
            .HasColumnName(nameof(TelegramMessage.SrcMessageId).ToSnakeCase());
        builder.HasOne(x => x.SrcMessage)
            .WithMany(x => x.TelegramMessages)
            .HasForeignKey(x => x.SrcMessageId);

        builder.HasIndex(nameof(TelegramMessage.ExtId), nameof(TelegramMessage.ChatId)).IsUnique();
    }
}