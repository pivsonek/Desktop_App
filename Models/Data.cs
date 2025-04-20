using System;
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
