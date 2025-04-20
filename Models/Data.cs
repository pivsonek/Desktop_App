namespace project.Models
{
    /// <summary>
    /// Represents a single row of data with frequency, temperature, and additional values.
    /// </summary>
    public class Data
    {
        /// <summary>
        /// Identifier of the data row.
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// Frequency value of the row.
        /// </summary>
        public double Frequency { get; init; }

        /// <summary>
        /// Temperature value of the row.
        /// </summary>
        public double Temperature { get; init; }

        /// <summary>
        /// Additional values associated with this row, stored as key-value pairs.
        /// </summary>
        public IReadOnlyDictionary<string, double> extraValues { get; init; }

        /// <summary>
        /// Initializes a new instance of the Data class.
        /// </summary>
        /// <param name="id">Unique row identifier.</param>
        /// <param name="freq">Frequency value.</param>
        /// <param name="temperature">Temperature value.</param>
        /// <param name="values">Dictionary of additional values.</param>
        public Data(int id, double freq, double temperature, Dictionary<string, double> values)
        {
            Id = id;
            Frequency = freq;
            Temperature = temperature;
            extraValues = values;
        }
    }
}
