using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniCast.Domain.Moodle;
using UniCast.Infrastructure.Persistence.Extensions;

namespace UniCast.Infrastructure.Persistence.EntityConfigurations;

public sealed class MoodleAccountEntityConfiguration : IEntityTypeConfiguration<MoodleAccount>
{
    public void Configure(EntityTypeBuilder<MoodleAccount> builder)
    {
        builder.ToTable(nameof(MoodleAccount).ToSnakeCase());

        builder.HasId();

        builder.Property(x => x.ExtId)
            .IsRequired()
            .HasColumnName(nameof(MoodleAccount.ExtId).ToSnakeCase());
        builder.HasIndex(x => x.ExtId).IsUnique();

        builder.Property(x => x.Username)
            .IsRequired()
            .HasColumnName(nameof(MoodleAccount.Username).ToSnakeCase());
        builder.HasIndex(x => x.Username).IsUnique();

        builder.Property(x => x.CurrentToken)
            .HasColumnName(nameof(MoodleAccount.CurrentToken).ToSnakeCase());

        builder.Property(x => x.StudentId)
            .HasDefaultIdConversion()
            .HasColumnName(nameof(MoodleAccount.StudentId).ToSnakeCase());

        builder.HasOne(x => x.Student)
            .WithOne(x => x.MoodleAccount)
            .HasForeignKey<MoodleAccount>(x => x.StudentId);
    }
}