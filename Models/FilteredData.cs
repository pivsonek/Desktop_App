using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace project.Models
{
    /// <summary>
    /// Represents a filtered dataset based on temperature or frequency.
    /// </summary>
    public class FilteredData : BaseData
    {
        /// <summary>
        /// Type of filter applied ("temperature" or "frequency").
        /// </summary>
        public string? FilterType { get; private set; }

        /// <summary>
        /// The numeric value used for filtering.
        /// </summary>
        public double FilterValue { get; private set; }

        /// <summary>
        /// The resulting filtered dataset.
        /// </summary>
        public IReadOnlyList<Data> Filtered { get; private set; }

        /// <summary>
        /// Constructs a new filtered dataset.
        /// </summary>
        /// <param name="filterType">The type of filter ("temperature" or "frequency").</param>
        /// <param name="filterValue">The value to filter by.</param>
        /// <param name="source">The source measurement data.</param>
        /// <exception cref="ArgumentException">Thrown if the filter type is invalid.</exception>
        public FilteredData(string filterType, double filterValue, MeasureData source)
        {
            FilterType = filterType.ToLowerInvariant();
            FilterValue = filterValue;

            Filtered = FilterType switch
            {
                "temperature" => source.GetDataByTemperature(filterValue).ToList(),
                "frequency" => source.GetDataByFrequency(filterValue).ToList(),
                _ => throw new ArgumentException("Invalid filter type.")
            };
        }

        /// <summary>
        /// Checks whether the filtered dataset contains any entries.
        /// </summary>
        public bool Any() => Filtered.Any();

        /// <summary>
        /// Converts the filtered data to a list of formatted text lines.
        /// </summary>
        /// <param name="maxRows">Maximum number of rows to include.</param>
        /// <returns>List of formatted string lines representing the filtered data.</returns>
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
