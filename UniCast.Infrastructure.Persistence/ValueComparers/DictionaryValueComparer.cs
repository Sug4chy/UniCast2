using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace UniCast.Infrastructure.Persistence.ValueComparers;

internal sealed class DictionaryValueComparer() : ValueComparer<Dictionary<string, string>>(
    equalsExpression: (dict1, dict2) => (dict1 == null && dict2 == null) || dict1!.SequenceEqual(dict2!),
    hashCodeExpression: dict => dict.Aggregate(0, (a, b) => HashCode.Combine(a, b.GetHashCode())),
    snapshotExpression: dict => dict.ToDictionary()
);