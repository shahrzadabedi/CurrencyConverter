﻿namespace CurrencyConverter.Infrastructure.Models;

public class CacheSettings
{
    public float SlidingExpirationInMinutes { get; set; }

    public bool Enabled { get; set; }
}
