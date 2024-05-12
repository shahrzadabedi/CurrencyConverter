using CurrencyConverter.API;
using CurrencyConverter.Application.Currencies;
using System;
using FluentAssertions;
using Xunit;
using CurrencyConverter.Infrastructure.Models;
using CurrencyConverter.Infrastructure;
using Microsoft.Extensions.Caching.Memory;

namespace CurrencyConverter.Test;

public class UpdateConfigurationTests
{
    [Fact]
    public void WhenUpdateConfiguration_ConvertSucceeds()
    {
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var cacheSettings = new CacheSettings { SlidingExpirationInMinutes = 15, Enabled = false };
        var currencyConverter = new CurrencyConverterService(new BfsShortestPathProvider(), cacheSettings, new CacheProvider(memoryCache, cacheSettings));

        Tuple<string, string, double>[] conversionValues =
        {
            new ("0", "1", 0.12),
            new ("0", "2", 1.568333),
            new ("0", "3", 6.1),
            new ("1", "3",3.1),
            new ("0", "5",5),
            new ("1", "4",0.5),
            new ("4", "5",1.2),
        };

        currencyConverter.UpdateConfiguration(conversionValues);

        currencyConverter.Convert("0", "3", 5).Should().Be(30.5);
    }
}

