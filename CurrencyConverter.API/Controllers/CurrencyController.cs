using CurrencyConverter.API.Extensions;
using CurrencyConverter.Application.Models;
using CurrencyConverter.Domain;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyConverter.Controllers;

[ApiController]
[Route("[controller]")]
public class CurrencyController : ControllerBase
{

    private readonly ICurrencyConverter _currencyConverter;

    public CurrencyController(ICurrencyConverter currencyConverter)
    {
        _currencyConverter = currencyConverter;
    }

    [HttpGet]
    public ActionResult<double> Convert(string fromCurrency, string toCurrency, double amount)
    {
        try
        {
            var result = _currencyConverter.Convert(fromCurrency, toCurrency, amount);

            return Result<double>.Success(result).ToActionResult();
        }
        catch (NoConversionAvailableException exception)
        {
            return Result<double>.InvalidOperationError(exception.Message).ToActionResult();
        }
        catch (Exception e)
        {
            return Result<double>.Error(e).ToActionResult();
        }
    }

    [HttpPost]
    public ActionResult<bool> UpdateConfigurations([FromBody] IEnumerable<Tuple<string, string, double>> conversionRates)
    {
        _currencyConverter.UpdateConfiguration(conversionRates);

        return Result<bool>.Success(true).ToActionResult();
    }

    [HttpDelete]
    public ActionResult<bool> ClearConfiguration()
    {
        _currencyConverter.ClearConfiguration();

        return Result<bool>.Success(true).ToActionResult();
    }
}

