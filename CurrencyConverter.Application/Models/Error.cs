using CurrencyConverter.Application.Enums;

namespace CurrencyConverter.Application.Models;

public class Error
{
    public string Code { get; set; }
    public HttpErrorCode HttpCode { get; set; }
    public string? Message { get; set; }
}
