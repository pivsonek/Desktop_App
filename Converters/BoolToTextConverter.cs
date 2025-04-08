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
