﻿using CurrencyConverter.API;
using CurrencyConverter.Application.Currencies;
using System;
using CurrencyConverter.Application.Models;
using CurrencyConverter.Infrastructure;
using CurrencyConverter.Infrastructure.Models;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Xunit;

namespace CurrencyConverter.Test;

public class ClearConfigurationTests
{
    [Fact]
    public void WhenClearConfiguration_ShouldSucceed()
    {
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var cacheSettings = new CacheSettings { SlidingExpirationInMinutes = 15 };
        var currencyConverter = new CurrencyConverterService(new BfsShortestPathProvider(), new CacheProvider(memoryCache,cacheSettings));

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

        currencyConverter.ClearConfiguration();

        Assert.Throws<NoConversionAvailableException>(() => currencyConverter.Convert("0", "3", 5));
    }
}

