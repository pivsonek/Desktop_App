using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace project.Models
{
    /// <summary>
    /// Třída reprezentující filtrovaná data
    /// </summary>
    public class FilteredData : BaseData
    {
        public string? FilterType { get; private set; } // "temperature" nebo "frequency"
        public double FilterValue { get; private set; }

        public IReadOnlyList<Data> Filtered { get; private set; }

        public FilteredData(string filterType, double filterValue, MeasureData source)
        {
            FilterType = filterType.ToLowerInvariant();
            FilterValue = filterValue;

            Filtered = FilterType switch
            {
                "temperature" => Filtered = source.GetDataByTemperature(filterValue).ToList(),
                "frequency" => Filtered = source.GetDataByFrequency(filterValue).ToList(),
                _ => throw new ArgumentException("Neplatný typ filtru.")
            };
        }

        public bool Any() => Filtered.Any();

        public override async Task<List<string>> MakeToStringAsync(int maxRows)
        {
            return await Task.Run(() =>
            {
                List<string> lines = new();

                var extraKeys = Filtered
                    .SelectMany(d => d.extraValues.Keys)
                    .Distinct()
                    .ToArray();

                if (maxRows <= 0)
                    maxRows = Filtered.Count;

                var header = GetHeader(extraKeys);
                var rows = BuildDataRows(Filtered, extraKeys, maxRows);
                var columnWidths = ComputeColumnWidths(header, rows);

                lines.Add(FormatRow(header, columnWidths));

                foreach (var row in rows)
                {
                    lines.Add(FormatRow(row, columnWidths));
                }

                return lines;
            });
        }
    }
}
