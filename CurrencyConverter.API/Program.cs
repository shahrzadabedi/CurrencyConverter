using CurrencyConverter.API;
using CurrencyConverter.Application.Currencies;
using CurrencyConverter.Domain;
using CurrencyConverter.Domain.Contracts;
using CurrencyConverter.Infrastructure;
using CurrencyConverter.Infrastructure.Models;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ICurrencyConverter, CurrencyConverterService>();
builder.Services.AddSingleton<IShortestPathProvider, BfsShortestPathProvider>();
builder.Services.AddSingleton<ICacheProvider, CacheProvider>();

builder.Services.Configure<CacheSettings>(builder.Configuration.GetSection(nameof(CacheSettings)));
builder.Services.AddSingleton(service => service.GetRequiredService<IOptions<CacheSettings>>().Value);

builder.Services.AddMemoryCache();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
