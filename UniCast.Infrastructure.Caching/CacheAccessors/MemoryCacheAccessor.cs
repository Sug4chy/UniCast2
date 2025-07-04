using Microsoft.Extensions.Caching.Memory;
using UniCast.Application.Abstractions.Cache;

namespace UniCast.Infrastructure.Caching.CacheAccessors;

internal sealed class MemoryCacheAccessor : ICacheAccessor
{
    private readonly IMemoryCache _cache;

    public MemoryCacheAccessor(IMemoryCache cache)
    {
        _cache = cache;
    }

    public ValueTask<T?> GetAsync<T>(string key, CancellationToken ct = default) 
        => ValueTask.FromResult(_cache.Get<T>(key));

    public ValueTask PutAsync<T>(string key, T value, CancellationToken ct = default)
    {
        _cache.Set(key, value);
        return ValueTask.CompletedTask;
    }
}