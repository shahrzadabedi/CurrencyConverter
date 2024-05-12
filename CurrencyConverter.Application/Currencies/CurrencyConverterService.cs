using CurrencyConverter.Application.Models;
using CurrencyConverter.Domain;
using CurrencyConverter.Domain.Contracts;
using CurrencyConverter.Infrastructure;
using CurrencyConverter.Infrastructure.Models;

namespace CurrencyConverter.Application.Currencies;

public class CurrencyConverterService : ICurrencyConverter
{
    private readonly IShortestPathProvider _shortestPathProvider;
    private readonly ICacheProvider _cacheProvider;
    private readonly CacheSettings _cacheSettings;

    public CurrencyConverterService(IShortestPathProvider shortestPathProvider,CacheSettings cacheSettings, ICacheProvider cacheProvider)
    {
        _shortestPathProvider = shortestPathProvider;
        _cacheSettings = cacheSettings;
        _cacheProvider = cacheProvider;
    }

    public void ClearConfiguration() => _shortestPathProvider.ClearConfiguration();

    public void UpdateConfiguration(IEnumerable<Tuple<string, string, double>> conversionRates) => _shortestPathProvider.UpdateConfiguration(conversionRates);

    public double Convert(string fromCurrency, string toCurrency, double amount)
    {
        try
        {
            var cacheId = $"{fromCurrency}_{toCurrency}";
            var cacheValue = _cacheSettings.Enabled? _cacheProvider.GetEntry<double?>(CacheDataType.ConversionRate, cacheId):null;

            if (cacheValue.HasValue)
                return cacheValue.Value * amount;

            var exchangeRate =  _shortestPathProvider.FindShortestPathWithConversionRate(fromCurrency,
                toCurrency).ConvertedValue!.Value;

            _cacheProvider.SetEntry(CacheDataType.ConversionRate, cacheId, exchangeRate);

            return exchangeRate*amount;
        }
        catch
        {
            throw new NoConversionAvailableException($"Cannot convert from {fromCurrency} to {toCurrency}");
        }
    }
}
