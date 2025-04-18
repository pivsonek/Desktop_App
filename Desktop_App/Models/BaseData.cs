using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.Models
{
    public abstract class BaseData
    {
        /// <summary>
        /// Creates header of the function
        /// </summary>
        /// <param name="extraKeys">key values for dictionary</param>
        /// <returns>List which represents header </returns>
        protected List<string> GetHeader(string[] extraKeys)
        {
            List<string> header = new() { "Frequency", "Temperature" };
            header.AddRange(extraKeys);
            return header;
        }

        /// <summary>
        /// Creates one row of data
        /// </summary>
        /// <param name="dataList"></param>
        /// <param name="extraKeys"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        protected List<List<string>> BuildDataRows(IEnumerable<Data> dataList, string[] extraKeys, int limit)
        {
            List<List<string>> rows = new();
            foreach (var d in dataList.Take(limit))
            {
                List<string> row = new() { d.Frequency.ToString("E2"), d.Temperature.ToString("E2") };

                foreach (var key in extraKeys)
                {
                    if (d.extraValues.TryGetValue(key, out double val))
                        row.Add(val.ToString("E2"));
                    else
                        row.Add("-");
                }

                rows.Add(row);
            }

            return rows;
        }

        /// <summary>
        /// Creates widths for formating
        /// </summary>
        /// <param name="header"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        protected int[] ComputeColumnWidths(List<string> header, List<List<string>> rows)
        {
            int[] widths = new int[header.Count];
            for (int i = 0; i < header.Count; i++)
                widths[i] = header[i].Length;

            foreach (var row in rows)
            {
                for (int i = 0; i < row.Count; i++)
                    widths[i] = Math.Max(widths[i], row[i].Length);
            }

            return widths;
        }

        /// <summary>
        /// Formats one row depending on the widths
        /// </summary>
        /// <param name="values"></param>
        /// <param name="widths"></param>
        /// <returns></returns>
        protected string FormatRow(List<string> values, int[] widths)
        {
            return string.Join(" \t", values.Select((val, i) => val.PadRight(widths[i])));
        }

        public abstract Task<List<string>> MakeToStringAsync(int maxRows);
    }
}
