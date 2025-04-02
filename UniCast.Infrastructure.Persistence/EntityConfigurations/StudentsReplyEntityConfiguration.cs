using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniCast.Domain.Messages.Entities;
using UniCast.Infrastructure.Persistence.Extensions;

namespace UniCast.Infrastructure.Persistence.EntityConfigurations;

public sealed class StudentsReplyEntityConfiguration : IEntityTypeConfiguration<StudentsReply>
{
    public void Configure(EntityTypeBuilder<StudentsReply> builder)
    {
        builder.ToTable(nameof(StudentsReply).ToSnakeCase());

        builder.HasId();

        builder.Property(x => x.ReplyText)
            .IsRequired()
            .HasColumnName(nameof(StudentsReply.ReplyText).ToSnakeCase());

        builder.Property(x => x.StudentId)
            .IsRequired()
            .HasDefaultIdConversion()
            .HasColumnName(nameof(StudentsReply.StudentId).ToSnakeCase());
        builder.HasOne(x => x.Student)
            .WithMany()
            .HasForeignKey(x => x.StudentId);

        builder.Property(x => x.MessageId)
            .IsRequired()
            .HasDefaultIdConversion()
            .HasColumnName(nameof(StudentsReply.MessageId).ToSnakeCase());
        builder.HasOne(x => x.Message)
            .WithMany()
            .HasForeignKey(x => x.MessageId);
    }
}