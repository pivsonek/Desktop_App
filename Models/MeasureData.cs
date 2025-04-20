using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.Models
{
    /// <summary>
    /// Represents a measurement consisting of multiple data records parsed from a file.
    /// </summary>
    public class MeasureData : BaseData
    {
        /// <summary>
        /// The name of the measurement.
        /// </summary>
        public string? Name { get; private set; }

        /// <summary>
        /// Read-only collection of loaded data.
        /// </summary>
        public IReadOnlyList<Data> FileData => _fileData;
        private readonly List<Data> _fileData = new();

        /// <summary>
        /// Determines whether any data has been loaded.
        /// </summary>
        public bool AnyData() => _fileData.Any();

        /// <summary>
        /// Filters the data based on the given temperature.
        /// </summary>
        public IEnumerable<Data> GetDataByTemperature(double temperature) =>
            _fileData.Where(d => d.Temperature == temperature);

        /// <summary>
        /// Filters the data based on the given frequency.
        /// </summary>
        public IEnumerable<Data> GetDataByFrequency(double frequency) =>
            _fileData.Where(d => d.Frequency == frequency);

        /// <summary>
        /// Loads measurement data from a given file.
        /// </summary>
        /// <param name="filePath">The full path to the data file.</param>
        /// <returns>True if data was successfully loaded, otherwise false.</returns>
        public async Task<bool> LoadData(string? filePath)
        {
            return await Task.Run(() =>
            {
                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                {
                    Console.WriteLine("Invalid file path!");
                    return false;
                }

                var lines = File.ReadAllLines(filePath);

                if (lines.Length < 5)
                {
                    Console.WriteLine("Insufficient data in file!");
                    return false;
                }

                int index = 0;
                Name = lines[index++];

                while (index < lines.Length && !lines[index].StartsWith(" ")) index++;
                if (index >= lines.Length) return false;

                string[] header = lines[index++].Split('\t').Select(h => h.Trim()).ToArray();

                int freqIndex = Array.FindIndex(header, h => h.Contains("Freq", StringComparison.OrdinalIgnoreCase));
                int tempIndex = Array.FindIndex(header, h => h.Contains("Temp", StringComparison.OrdinalIgnoreCase));

                if (freqIndex == -1 || tempIndex == -1)
                {
                    Console.WriteLine("Missing required columns in header!");
                    return false;
                }

                for (int i = index; i < lines.Length; i++)
                {
                    string[] values = lines[i].Split('\t');

                    if (values.Length < header.Length ||
                        !double.TryParse(values[freqIndex], NumberStyles.Any, CultureInfo.InvariantCulture, out double freq) ||
                        !double.TryParse(values[tempIndex], NumberStyles.Any, CultureInfo.InvariantCulture, out double temp))
                    {
                        continue;
                    }

                    var extraData = new Dictionary<string, double>();
                    for (int j = 0; j < header.Length && j < values.Length; j++)
                    {
                        if (j != freqIndex && j != tempIndex &&
                            double.TryParse(values[j], NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
                        {
                            extraData[header[j]] = value;
                        }
                    }

                    _fileData.Add(new Data(i, freq, temp, extraData));
                }

                return true;
            });
        }

        /// <summary>
        /// Converts a portion of the loaded data into formatted text lines.
        /// </summary>
        /// <param name="maxRows">Maximum number of data rows to include in the output.</param>
        /// <returns>Formatted lines representing the data preview.</returns>
        public async override Task<List<string>> MakeToStringAsync(int maxRows = 100)
        {
            return await Task.Run(() =>
            {
                List<string> lines = new();
                lines.Add($"Measurement: {Name}");

                var extraKeys = _fileData
                    .SelectMany(d => d.extraValues.Keys)
                    .Distinct()
                    .ToArray();

                var header = GetHeader(extraKeys);
                var rows = BuildDataRows(_fileData, extraKeys, maxRows);
                var columnWidths = ComputeColumnWidths(header, rows);

                lines.Add(FormatRow(header, columnWidths));

                foreach (var row in rows)
                {
                    lines.Add(FormatRow(row, columnWidths));
                }

                if (_fileData.Count > maxRows)
                {
                    lines.Add($"\n... a {_fileData.Count - maxRows} dalších řádek neukázáno.");
                }

                return lines;
            });
        }
    }
}
