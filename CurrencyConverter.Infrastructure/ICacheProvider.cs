using CurrencyConverter.Domain;

namespace CurrencyConverter.Infrastructure;
public interface ICacheProvider
{
    void SetEntry(CacheDataType dataType, string id, object value);

    T? GetEntry<T>(CacheDataType dataType, string id);
}
