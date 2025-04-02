using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniCast.Domain.Students.Entities;
using UniCast.Domain.Students.ValueObjects;
using UniCast.Infrastructure.Persistence.Extensions;

namespace UniCast.Infrastructure.Persistence.EntityConfigurations;

public sealed class AcademicGroupEntityConfiguration : IEntityTypeConfiguration<AcademicGroup>
{
    public void Configure(EntityTypeBuilder<AcademicGroup> builder)
    {
        builder.ToTable(nameof(AcademicGroup).ToSnakeCase());

        builder.HasId();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasConversion(
                name => name.ToString(),
                str => AcademicGroupName.From(str)
            )
            .HasColumnName(nameof(AcademicGroup.Name).ToSnakeCase());
        builder.HasIndex(x => x.Name).IsUnique();

        builder.HasMany(x => x.Students)
            .WithOne(x => x.Group)
            .HasForeignKey(x => x.GroupId);
    }
}