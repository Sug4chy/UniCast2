using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniCast.Domain.Telegram.Entities;
using UniCast.Domain.Telegram.ValueObjects.Enums;
using UniCast.Infrastructure.Persistence.Extensions;
using UniCast.Infrastructure.Persistence.ValueComparers;

namespace UniCast.Infrastructure.Persistence.EntityConfigurations;

public sealed class TelegramChatEntityConfiguration : IEntityTypeConfiguration<TelegramChat>
{
    public void Configure(EntityTypeBuilder<TelegramChat> builder)
    {
        builder.ToTable(nameof(TelegramChat).ToSnakeCase());

        builder.UseTphMappingStrategy();

        builder.HasId();

        builder.Property(x => x.Title)
            .IsRequired()
            .HasColumnName(nameof(TelegramChat.Title).ToSnakeCase());
        builder.HasIndex(x => x.Title).IsUnique();

        builder.Property(x => x.ExtId)
            .IsRequired()
            .HasColumnName(nameof(TelegramChat.ExtId).ToSnakeCase());
        builder.HasIndex(x => x.ExtId).IsUnique();

        builder.Property(x => x.Type)
            .IsRequired()
            .HasColumnName(nameof(TelegramChat.Type).ToSnakeCase());
        builder.HasDiscriminator(x => x.Type)
            .HasValue<PrivateTelegramChat>(TelegramChatType.Private)
            .HasValue<TelegramChannel>(TelegramChatType.Channel);

        builder.HasMany(x => x.Messages)
            .WithOne(x => x.Chat)
            .HasForeignKey(x => x.ChatId);
    }

    public sealed class PrivateTelegramChatEntityConfiguration : IEntityTypeConfiguration<PrivateTelegramChat>
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.General);

        public void Configure(EntityTypeBuilder<PrivateTelegramChat> builder)
        {
            builder.Property(x => x.StudentId)
                .HasDefaultIdConversion()
                .HasColumnName(nameof(PrivateTelegramChat.StudentId).ToSnakeCase());
            builder.HasIndex(x => x.StudentId).IsUnique();

            builder.HasOne(x => x.Student)
                .WithOne(x => x.TelegramChat)
                .HasForeignKey<PrivateTelegramChat>(x => x.StudentId);

            builder.Property(x => x.CurrentScenario)
                .HasColumnName(nameof(PrivateTelegramChat.CurrentScenario).ToSnakeCase());

            builder.Property(x => x.CurrentState)
                .HasColumnName(nameof(PrivateTelegramChat.CurrentState).ToSnakeCase());

            builder.Property(x => x.CurrentScenarioArgs)
                .HasColumnType("jsonb")
                .HasConversion<string>(
                    v => JsonSerializer.Serialize(v, JsonSerializerOptions),
                    v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, JsonSerializerOptions)!,
                    new DictionaryValueComparer())
                .HasColumnName(nameof(PrivateTelegramChat.CurrentScenarioArgs).ToSnakeCase());
        }
    }

    public sealed class TelegramChannelEntityConfiguration : IEntityTypeConfiguration<TelegramChannel>
    {
        public void Configure(EntityTypeBuilder<TelegramChannel> builder)
        {
            builder.Property(x => x.GroupId)
                .HasDefaultIdConversion()
                .HasColumnName(nameof(TelegramChannel.GroupId).ToSnakeCase());
            builder.HasIndex(x => x.GroupId).IsUnique();

            builder.HasOne(x => x.AcademicGroup)
                .WithOne(x => x.TelegramChannel)
                .HasForeignKey<TelegramChannel>(x => x.GroupId);
        }
    }
}