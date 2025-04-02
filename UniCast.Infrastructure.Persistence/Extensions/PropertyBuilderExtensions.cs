using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniCast.Domain.Common.ValueObjects;

namespace UniCast.Infrastructure.Persistence.Extensions;

public static class PropertyBuilderExtensions
{
    public static PropertyBuilder<IdOf<TEntity>> HasDefaultIdConversion<TEntity>(
        this PropertyBuilder<IdOf<TEntity>> propertyBuilder)
        where TEntity : Entity<IdOf<TEntity>>
        => propertyBuilder.HasConversion<Guid>(
            id => id,
            id => IdOf<TEntity>.From(id)
        );

    public static PropertyBuilder<IdOf<TEntity>?> HasDefaultIdConversion<TEntity>(
        this PropertyBuilder<IdOf<TEntity>?> propertyBuilder)
        where TEntity : Entity<IdOf<TEntity>>
        => propertyBuilder.HasConversion<Guid>(
            id => id ?? default,
            id => IdOf<TEntity>.From(id)
        );
}