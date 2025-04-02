using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniCast.Domain.Students.Entities;
using UniCast.Domain.Students.ValueObjects;
using UniCast.Infrastructure.Persistence.Extensions;

namespace UniCast.Infrastructure.Persistence.EntityConfigurations;

public sealed class StudentEntityConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable(nameof(Student).ToSnakeCase());

        builder.HasId();

        builder.Property(x => x.FullName)
            .IsRequired()
            .HasConversion<string>(
                name => name.ToString(),
                str => StudentFullName.From(str)
            )
            .HasColumnName(nameof(Student.FullName).ToSnakeCase());

        builder.Property(x => x.GroupId)
            .IsRequired()
            .HasDefaultIdConversion()
            .HasColumnName(nameof(Student.GroupId).ToSnakeCase());

        builder.HasOne(x => x.Group)
            .WithMany(x => x.Students)
            .HasForeignKey(x => x.GroupId);
    }
}