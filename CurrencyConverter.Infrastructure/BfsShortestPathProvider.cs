using System.Collections.Concurrent;
using CurrencyConverter.Domain.Contracts;

namespace CurrencyConverter.API;

public class Vertex
{
    public string Title { get; set; }

    public bool IsVisited { get; set; } = false;
}

public class Calculate
{
    public string Title { get; set; }

    public double ConversionValue { get; set; }
}

public class BfsShortestPathProvider : IShortestPathProvider
{
    private List<Tuple<string, string, double>> _edges = new();

    private ConcurrentDictionary<string, List<Calculate>> _adjacencyLists = new();

    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, Tuple<string, int, int?>>>
        _sourceShortestPathMatrices = new();

    private List<Vertex> _vertices = new();

    private readonly Queue<string> _queue = new();

    public void ClearConfiguration()
    {
        lock (_edges)
        {
            _edges.Clear();

            _adjacencyLists.Clear();

            _sourceShortestPathMatrices.Clear();

            _vertices.Clear();
        }
    }

    public void UpdateConfiguration(IEnumerable<Tuple<string, string, double>> conversionRates)
    {
        lock (_edges)
        {
            _edges = conversionRates.ToList();

            BuildAdjacencyLists();
        }
    }

    public void ResetVisited() => _vertices.ForEach(v => v.IsVisited = false);

    public ConcurrentDictionary<string, Tuple<string, int, int?>> CalculateCosts(string source)
    {
        ConcurrentDictionary<string, Tuple<string, int, int?>> costs = new();

        lock (_edges)
        {
            ResetVisited();

            var findSourceVertex = _vertices.FirstOrDefault(v => v.Title == source && !v.IsVisited);
            findSourceVertex!.IsVisited = true;

            VisitVertex(source, costs);

            while (_queue.Count != 0)
            {
                var currentVertex = _queue!.Dequeue();

                costs[currentVertex] =
                    new Tuple<string, int, int?>(costs[currentVertex].Item1, costs[currentVertex].Item2,
                        costs[currentVertex].Item2 + 1);

                VisitVertex(currentVertex, costs);
            }
        }

        return costs;

    }

    public ShortestPath FindShortestPath(string source, string destination)
    {
        if (!_vertices.Select(v => v.Title).Contains(source))
            throw new Exception("Source symbol is not valid");

        if (!_vertices.Select(v => v.Title).Contains(destination))
            throw new Exception("Destination symbol is not valid");

        ConcurrentDictionary<string, Tuple<string, int, int?>> costs;

        if (!_sourceShortestPathMatrices.ContainsKey(source))
        {
            costs = CalculateCosts(source);

            _sourceShortestPathMatrices[source] = costs;
        }
        else
            costs = _sourceShortestPathMatrices[source];

        var reversePath = new List<string> { destination };

        if (!costs.ContainsKey(destination))
            throw new Exception("There are no paths from source node to this destination.");

        var parent = costs[destination].Item1;
        reversePath.Add(parent);

        while (costs.ContainsKey(parent))
        {
            parent = costs[parent].Item1;

            reversePath.Add(parent);
        }

        reversePath.Reverse();

        return new ShortestPath()
        {
            Path = reversePath,
            Cost = reversePath.Count - 1
        };
    }

    public ShortestPath FindShortestPathWithConversionRate(string source, string destination)
    {
        if (!_vertices.Select(v => v.Title).Contains(source))
            throw new Exception("Source symbol is not valid");

        if (!_vertices.Select(v => v.Title).Contains(destination))
            throw new Exception("Destination symbol is not valid");

        ConcurrentDictionary<string, Tuple<string, int, int?>> costs;

        if (!_sourceShortestPathMatrices.ContainsKey(source))
        {
            costs = CalculateCosts(source);

            _sourceShortestPathMatrices[source] = costs;
        }
        else
            costs = _sourceShortestPathMatrices[source];

        var reversePath = new List<string> { destination };

        if (!costs.ContainsKey(destination))
            throw new Exception("There are no paths from source node to this destination.");

        var parent = costs[destination].Item1;
        reversePath.Add(parent);

        var conversionValue = _adjacencyLists[destination]
            .FirstOrDefault(a => a.Title == parent)!.ConversionValue;

        while (costs.ContainsKey(parent))
        {
            var copy = parent;
            parent = costs[parent].Item1;
            conversionValue *= _adjacencyLists[copy]
                .FirstOrDefault(a => a.Title == parent)!.ConversionValue;
            reversePath.Add(parent);
        }

        reversePath.Reverse();

        return new ShortestPath()
        {
            Path = reversePath,
            Cost = reversePath.Count - 1,
            ConvertedValue = 1 / conversionValue
        };
    }

    private void BuildAdjacencyLists()
    {
        var distinctVertices = new List<string>();
        var listOfAdjacencyLists = new ConcurrentDictionary<string, List<Calculate>>();

        foreach (var tuple in _edges)
        {
            var comparableItem = tuple.Item1;
            if (!distinctVertices.Contains(tuple.Item1))
            {
                distinctVertices.Add(comparableItem);
            }

            var adjacencyList = new List<Calculate>
            {
                new Calculate()
                {
                    Title = tuple.Item2,
                    ConversionValue = tuple.Item3
                }
            };

            if (!listOfAdjacencyLists.ContainsKey(comparableItem))
            {
                foreach (var tuple2 in _edges)
                {
                    if (!Equals(tuple2, tuple) && tuple2.Item1 == comparableItem)
                        adjacencyList.Add(new Calculate() { Title = tuple2.Item2, ConversionValue = tuple2.Item3 });
                    else if (!Equals(tuple2, tuple) && tuple2.Item2 == comparableItem)
                        adjacencyList.Add(new Calculate()
                        {
                            Title = tuple2.Item1,
                            ConversionValue = tuple2.Item3
                        });
                }

                listOfAdjacencyLists[comparableItem] = adjacencyList;
            }

            comparableItem = tuple.Item2;
            if (!distinctVertices.Contains(tuple.Item2))
            {
                distinctVertices.Add(comparableItem);
            }

            adjacencyList = new List<Calculate>
            {
                new Calculate()
                {
                    Title = tuple.Item1,
                    ConversionValue = 1 / tuple.Item3
                }
            };

            if (!listOfAdjacencyLists.ContainsKey(comparableItem))
            {
                foreach (var tuple2 in _edges)
                {
                    if (tuple2 != tuple && tuple2.Item1 == comparableItem &&
                        !distinctVertices.Contains(tuple2.Item2))
                        adjacencyList.Add(new Calculate()
                        {
                            Title = tuple2.Item2,
                            ConversionValue = tuple2.Item3
                        });

                    else if (tuple2 != tuple && tuple2.Item2 == comparableItem &&
                             !distinctVertices.Contains(tuple2.Item1))
                        adjacencyList.Add(new Calculate()
                        {
                            Title = tuple2.Item1,
                            ConversionValue = tuple2.Item3
                        });
                }

                listOfAdjacencyLists[comparableItem] = adjacencyList;
            }
        }

        _adjacencyLists = listOfAdjacencyLists;
        _vertices = distinctVertices
                                .Select(v => new Vertex() { IsVisited = false, Title = v })
                                .ToList();
    }

    private void VisitVertex(string currentVertex, ConcurrentDictionary<string, Tuple<string, int, int?>> costs)
    {
        foreach (var neighbor in _adjacencyLists[currentVertex])
        {
            var findVertex = _vertices.FirstOrDefault(v => v.Title == neighbor.Title && !v.IsVisited);
            if (findVertex != null)
            {
                if (!costs.ContainsKey(neighbor.Title))
                {
                    if (costs.ContainsKey(currentVertex))
                        costs[neighbor.Title] =
                            new Tuple<string, int, int?>(currentVertex, costs[currentVertex].Item3!.Value, null);
                    else
                        costs[neighbor.Title] = new Tuple<string, int, int?>(currentVertex, 0, 1);
                }

                _queue.Enqueue(neighbor.Title);

                findVertex.IsVisited = true;
            }
        }
    }
}
