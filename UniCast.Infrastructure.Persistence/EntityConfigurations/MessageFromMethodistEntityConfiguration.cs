using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniCast.Domain.Messages.Entities;
using UniCast.Domain.Students.Entities;
using UniCast.Infrastructure.Persistence.Extensions;
using UniCast.Infrastructure.Persistence.Entities;

namespace UniCast.Infrastructure.Persistence.EntityConfigurations;

public sealed class MessageFromMethodistEntityConfiguration : IEntityTypeConfiguration<MessageFromMethodist>
{
    public void Configure(EntityTypeBuilder<MessageFromMethodist> builder)
    {
        builder.ToTable(nameof(MessageFromMethodist).ToSnakeCase());

        builder.HasId();

        builder.Property(x => x.Body)
            .IsRequired()
            .HasColumnName(nameof(MessageFromMethodist.Body).ToSnakeCase());

        builder.Property(x => x.SenderUsername)
            .IsRequired()
            .HasColumnName(nameof(MessageFromMethodist.SenderUsername).ToSnakeCase());

        builder.HasMany(x => x.Students)
            .WithMany(x => x.Messages)
            .UsingEntity<MessageFromMethodistStudent>(
                r => r.HasOne<Student>().WithMany().HasForeignKey(x => x.StudentId),
                l => l.HasOne<MessageFromMethodist>().WithMany().HasForeignKey(x => x.MessageId),
                j =>
                {
                    j.ToTable(nameof(MessageFromMethodistStudent).ToSnakeCase());

                    j.HasKey(nameof(MessageFromMethodistStudent.StudentId),
                        nameof(MessageFromMethodistStudent.MessageId));

                    j.Property(x => x.StudentId)
                        .IsRequired()
                        .HasColumnName(nameof(MessageFromMethodistStudent.StudentId).ToSnakeCase());

                    j.Property(x => x.MessageId)
                        .IsRequired()
                        .HasColumnName(nameof(MessageFromMethodistStudent.MessageId).ToSnakeCase());
                });
    }
}