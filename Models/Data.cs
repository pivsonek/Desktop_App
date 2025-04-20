<<<<<<< HEAD
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.Models
{
    /// <summary>
    /// Trida reprezentujici jeden radek dat
    /// </summary>
    public class Data
    {
        /// <summary>
        /// Identifikator daneho radku
        /// </summary>
        public int Id { get; init; }
        
        /// <summary>
        /// frekvence pro dany radek
        /// </summary>
        public double Frequency { get; init; }

        /// <summary>
        /// teplota pro dany radek
        /// </summary>
        public double Temperature { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyDictionary<string, double> extraValues { get; init; }

        /// <summary>
        /// Init funkce pro jeden radek dat
        /// </summary>
        /// <param name="id"></param>
        /// <param name="freq"></param>
        /// <param name="temperature"></param>
        /// <param name="values"></param>
        public Data(int id, double freq, double temperature, Dictionary<string, double> values)
        {
            Id = id;
            Frequency = freq;
            Temperature = temperature;
            extraValues = values;
        }
    }
}
=======
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
>>>>>>> export-w-graphs
