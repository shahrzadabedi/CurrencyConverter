using System.Collections.Concurrent;

namespace CurrencyConverter.Domain.Contracts;


public class ShortestPath
{
    public List<string> Path { get; set; }

    public int Cost { get; set; }

    public double? ConvertedValue { get; set; }
}

public interface IShortestPathProvider
{
    void ClearConfiguration();

    void UpdateConfiguration(IEnumerable<Tuple<string, string, double>> conversionRates);

    ConcurrentDictionary<string, Tuple<string, int, int?>> CalculateCosts(string source);

    ShortestPath FindShortestPath(string source, string destination);

    ShortestPath FindShortestPathWithConversionRate(string source, string destination);
}

