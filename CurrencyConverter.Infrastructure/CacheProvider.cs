using CurrencyConverter.Domain;
using CurrencyConverter.Infrastructure.Models;
using Microsoft.Extensions.Caching.Memory;

namespace CurrencyConverter.Infrastructure;

public class CacheProvider : ICacheProvider
{
    private readonly IMemoryCache _memoryCache;
    private readonly MemoryCacheEntryOptions _cacheEntryOptions;

    public CacheProvider(IMemoryCache memoryCache, CacheSettings cacheSettings)
    {
        _memoryCache = memoryCache;
        _cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(cacheSettings.SlidingExpirationInMinutes));
    }

    public void SetEntry(CacheDataType dataType, string id, object value)
    {
        var cacheKey = GetKeyName(dataType, id);

        _memoryCache.Set(cacheKey, value, _cacheEntryOptions);
    }

    public T? GetEntry<T>(CacheDataType dataType, string id)
    {
        var cacheKey = GetKeyName(dataType, id);

        _memoryCache.TryGetValue(cacheKey, out T result);

        return result;
    }

    private string GetKeyName(CacheDataType dataType, string id) => $"{dataType}_{id}";
}
