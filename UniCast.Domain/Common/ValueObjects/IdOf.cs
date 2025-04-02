using CSharpFunctionalExtensions;

namespace UniCast.Domain.Common.ValueObjects;

/// <summary>
/// Универсальный типизированный идентификатор сущности
/// </summary>
/// <typeparam name="TEntity">Тип сущности, к которой относится этот идентификатор</typeparam>
public readonly record struct IdOf<TEntity> : IComparable<IdOf<TEntity>>
    where TEntity : Entity<IdOf<TEntity>>
{
    private readonly Guid _value;

    private IdOf(Guid value)
    {
        _value = value;
    }

    /// <summary>
    /// Создаёт новый случайный идентификатор
    /// </summary>
    public static IdOf<TEntity> New()
        => new(Guid.NewGuid());

    /// <summary>
    /// Создаёт новый идентификатор на основе переданного Guid
    /// </summary>
    public static IdOf<TEntity> From(Guid value)
        => new(value);

    public static implicit operator Guid(IdOf<TEntity> studentId)
        => studentId._value;

    public int CompareTo(IdOf<TEntity> other)
    {
        return _value.CompareTo(other._value);
    }
}