<<<<<<< HEAD
using System;
using System.Globalization; // Pro podporu lokalizace (kultura převodu)
using Microsoft.Maui.Controls;

namespace project.Converters
{
    /// <summary>
    /// Převádí hodnotu bool na odpovídající text "Zmenšit" nebo "Zvětšit".
    /// Používá se pro tlačítka, která mění velikost grafu.
    /// </summary>
    public class BoolToTextConverter : IValueConverter
    {
        /// <summary>
        /// Převádí hodnotu typu bool na odpovídající text.
        /// </summary>
        /// <param name="value">Vstupní hodnota (true = rozbalený, false = sbalený).</param>
        /// <param name="targetType">Cílový typ (nepoužívá se).</param>
        /// <param name="parameter">Nepoužitý volitelný parametr.</param>
        /// <param name="culture">Kultura (nepoužívá se).</param>
        /// <returns>Vrací řetězec "Zmenšit", pokud je prvek rozbalený, jinak "Zvětšit".</returns>
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is bool isExpanded && isExpanded ? "Zmenšit" : "Zvětšit";
        }

        /// <summary>
        /// Zpětná konverze není podporována.
        /// </summary>
        /// <param name="value">Vstupní hodnota.</param>
        /// <param name="targetType">Cílový typ.</param>
        /// <param name="parameter">Volitelný parametr.</param>
        /// <param name="culture">Kultura.</param>
        /// <returns>Vrací Binding.DoNothing, což zabraňuje chybám při zpětné konverzi.</returns>
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return Binding.DoNothing; // Zabrání pokusu o zpětnou konverzi, protože není potřeba
        }
    }
}
=======
using System.Globalization;

namespace project.Converters
{
    /// <summary>
    /// Converts a boolean value to a corresponding text label.
    /// Used for buttons that toggle graph size.
    /// </summary>
    public class BoolToTextConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean to a string. Returns "Collapse" if true, otherwise "Expand".
        /// </summary>
        /// <param name="value">The input value (true = expanded, false = collapsed).</param>
        /// <param name="targetType">The target binding type (unused).</param>
        /// <param name="parameter">Optional parameter (unused).</param>
        /// <param name="culture">The culture info (unused).</param>
        /// <returns>Returns "Collapse" if expanded, otherwise "Expand".</returns>
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is bool isExpanded && isExpanded ? "Zmenšit" : "Zvětšit";
        }

        /// <summary>
        /// ConvertBack is not supported for this converter.
        /// </summary>
        /// <param name="value">The input value.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">Optional parameter.</param>
        /// <param name="culture">The culture info.</param>
        /// <returns>Returns Binding.DoNothing to prevent back-conversion.</returns>
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
>>>>>>> export-w-graphs
