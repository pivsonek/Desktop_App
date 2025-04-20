using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;

namespace project.Models;

/// <summary>
/// Provides functionality to generate graphs from data records.
/// </summary>
public static class GraphGenerator
{
    /// <summary>
    /// Creates a graph based on the provided dataset and selected value key.
    /// </summary>
    /// <param name="name">The name of the graph.</param>
    /// <param name="data">The data source used to generate the graph.</param>
    /// <param name="extraValueKey">The key for the Y-axis value from extra data.</param>
    /// <param name="xLabel">The label for the X-axis.</param>
    /// <param name="yLabel">The label for the Y-axis.</param>
    /// <returns>A new GraphModel object with generated series and axis configuration.</returns>
    public static GraphModel CreateGraph(string name, IEnumerable<Data> data, string extraValueKey, string xLabel = "Frekvence [Hz]", string yLabel = "Hodnota")
    {
        var groups = data
            .Where(d => d.extraValues.ContainsKey(extraValueKey))
            .GroupBy(d => d.Temperature)
            .OrderBy(g => g.Key);

        var series = new List<ISeries>();

        foreach (var group in groups)
        {
            var points = group
                .OrderBy(d => d.Frequency)
                .Select(d => new ObservablePoint(d.Frequency, d.extraValues[extraValueKey]))
                .ToList();

            series.Add(new LineSeries<ObservablePoint>
            {
                Values = points,
                Name = $"{group.Key} °C",
                GeometrySize = 5
            });
        }

        return new GraphModel
        {
            Name = name,
            Series = series.ToArray(),
            XAxes = new[] { new Axis { Name = xLabel } },
            YAxes = new[] { new Axis { Name = yLabel } },
            IsVisible = true,
            IsExpanded = false
        };
    }
}
