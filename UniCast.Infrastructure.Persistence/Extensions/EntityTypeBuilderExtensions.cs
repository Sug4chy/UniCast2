using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniCast.Domain.Common.ValueObjects;

namespace UniCast.Infrastructure.Persistence.Extensions;

public static class EntityTypeBuilderExtensions
{
    public static void HasId<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : Entity<IdOf<TEntity>>
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .IsRequired()
            .ValueGeneratedNever()
            .HasConversion<Guid>(
                id => id,
                id => IdOf<TEntity>.From(id)
            )
            .HasColumnName("id");
    }
}