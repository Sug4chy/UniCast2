using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniCast.Domain.Telegram.Entities;
using UniCast.Infrastructure.Persistence.Extensions;
using UniCast.Infrastructure.Persistence.ValueComparers;

namespace UniCast.Infrastructure.Persistence.EntityConfigurations;

public sealed class TelegramChatEntityConfiguration : IEntityTypeConfiguration<TelegramChat>
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.General);

    public void Configure(EntityTypeBuilder<TelegramChat> builder)
    {
        builder.ToTable(nameof(TelegramChat).ToSnakeCase());

        builder.HasId();

        builder.Property(x => x.Title)
            .IsRequired()
            .HasColumnName(nameof(TelegramChat.Title).ToSnakeCase());
        builder.HasIndex(x => x.Title).IsUnique();

        builder.Property(x => x.ExtId)
            .IsRequired()
            .HasColumnName(nameof(TelegramChat.ExtId).ToSnakeCase());
        builder.HasIndex(x => x.ExtId).IsUnique();

        builder.Property(x => x.StudentId)
            .HasDefaultIdConversion()
            .HasColumnName(nameof(TelegramChat.StudentId).ToSnakeCase());
        builder.HasIndex(x => x.StudentId).IsUnique();

        builder.HasOne(x => x.Student)
            .WithOne(x => x.TelegramChat)
            .HasForeignKey<TelegramChat>(x => x.StudentId);

        builder.Property(x => x.CurrentScenario)
            .HasColumnName(nameof(TelegramChat.CurrentScenario).ToSnakeCase());

        builder.Property(x => x.CurrentState)
            .HasColumnName(nameof(TelegramChat.CurrentState).ToSnakeCase());

        builder.Property(x => x.CurrentScenarioArgs)
            .HasColumnType("jsonb")
            .HasConversion<string>(
                convertToProviderExpression: v => JsonSerializer.Serialize(v, JsonSerializerOptions),
                convertFromProviderExpression: v =>
                    JsonSerializer.Deserialize<Dictionary<string, string>>(v, JsonSerializerOptions)!,
                valueComparer: new DictionaryValueComparer())
            .HasColumnName(nameof(TelegramChat.CurrentScenarioArgs).ToSnakeCase());

        builder.HasMany(x => x.Messages)
            .WithOne(x => x.Chat)
            .HasForeignKey(x => x.ChatId);
    }
}