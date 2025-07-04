namespace UniCast.Application.Abstractions.Cache;

public interface ICacheAccessor
{
    ValueTask<T?> GetAsync<T>(string key, CancellationToken ct = default);
    ValueTask PutAsync<T>(string key, T value, CancellationToken ct = default);
}