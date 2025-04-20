using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using project.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace project.Models;

/// <summary>
/// Generátor grafů.
/// </summary>
public static class GraphGenerator
{
    /// <summary>
    /// Vytvoří graf na základě zadaných dat.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="data"></param>
    /// <param name="extraValueKey"></param>
    /// <param name="xLabel"></param>
    /// <param name="yLabel"></param>
    /// <returns></returns>
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
