<<<<<<< HEAD
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.Globalization;

namespace project.Converters
{
    /// <summary>
    /// Konvertor, který převádí boolean hodnotu (např. jestli je graf rozbalený)
    /// na hodnotu typu ScrollBarVisibility.
    /// Používá se k řízení viditelnosti scroll baru v hlavní oblasti grafů.
    /// </summary>
    public class BoolToScrollVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Konverze z boolean na ScrollBarVisibility.
        /// true = scroll bar se skryje (ScrollBarVisibility.Never),
        /// false = zobrazí se podle potřeby (ScrollBarVisibility.Default).
        /// </summary>
        /// <param name="value">Hodnota typu bool – např. IsAnyGraphExpanded.</param>
        /// <param name="targetType">Typ výstupu (očekává se ScrollBarVisibility).</param>
        /// <param name="parameter">Nepoužívá se.</param>
        /// <param name="culture">Nepoužívá se.</param>
        /// <returns>ScrollBarVisibility odpovídající boolean hodnotě.</returns>
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isExpanded)
            {
                // Pokud je graf rozbalený, skryjeme scroll bar
                return isExpanded ? ScrollBarVisibility.Never : ScrollBarVisibility.Default;
            }

            // Výchozí hodnota, pokud vstup není boolean
            return ScrollBarVisibility.Default;
        }

        /// <summary>
        /// Zpětná konverze – zde ponechána jako pas-through.
        /// Většinou se v praxi nevyužívá.
        /// </summary>
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value!;
        }
    }
}
=======
using System.Globalization;

namespace project.Converters
{
    /// <summary>
    /// Converts a boolean value to ScrollBarVisibility.
    /// Used to control the scrollbar in the main graph area.
    /// </summary>
    public class BoolToScrollVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean value to ScrollBarVisibility.
        /// True = ScrollBarVisibility.Never, False = ScrollBarVisibility.Default.
        /// </summary>
        /// <param name="value">Boolean input – typically IsAnyGraphExpanded.</param>
        /// <param name="targetType">The expected output type (ScrollBarVisibility).</param>
        /// <param name="parameter">Unused.</param>
        /// <param name="culture">Unused.</param>
        /// <returns>Corresponding ScrollBarVisibility value.</returns>
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isExpanded)
            {
                return isExpanded ? ScrollBarVisibility.Never : ScrollBarVisibility.Default;
            }

            return ScrollBarVisibility.Default;
        }

        /// <summary>
        /// Pass-through backward conversion (rarely used).
        /// </summary>
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value!;
        }
    }
}
>>>>>>> export-w-graphs
