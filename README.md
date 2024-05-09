In the Currency Converter project, we have an interface that lets us configure, save and clear exchange rates:
```
public interface ICurrencyConverter
{
    void ClearConfiguration();

    void UpdateConfiguration(IEnumerable<Tuple<string, string, double>> conversionRates);

    double Convert(string fromCurrency, string toCurrency, double amount);
    }
```

I used BFS algorithm to traverse the graph. To represent the graph, I make an adjacency list for each vertex using BuildAdjacencyLists() method in 
BfsShortestPathProvider class that implements IShortestPathProvider interface.
1. UpdateConfiguration method:
   It adjusts and sets up adjacency lists for all vertices and also makes a vertices list for later check.
The Convert method first traverses the whole graph using BFS algorithm. Meanwhile it builds a dictionary that 
keeps (the parent of each vertex, the cost to reach the parent, the cost to reach the current vertex).
After the whole graph is traversed it then uses this dictionary and starts from destination verex to finally reach the source vertex.

2. ClearConfiguration method:
   this method clears all adjustments
3.UpdateConfiguration method:
   This method sets up the adjacency lists and the list of vertices for later traversal.
