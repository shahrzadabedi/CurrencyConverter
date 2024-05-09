namespace CurrencyConverter.Application.Models;

public class NoConversionAvailableException : Exception
{
    public NoConversionAvailableException(): base("No conversion available.")
    {
    }

    public NoConversionAvailableException(string message): base(message) { }
}

