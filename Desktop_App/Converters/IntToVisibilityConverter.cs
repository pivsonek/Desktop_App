using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace project.Converters
{
    /// <summary>
    /// Převádí hodnotu int na hodnotu bool pro ověření, zda jsou otevřené nějaké taby.
    /// Používá se pro záložky nahraných souborů.
    /// </summary>
    public class IntToVisibilityConverter : IValueConverter
    {

        public static void ForceCompile() { }
        /// <summary>
        /// Převádí int hodnotu (počet otevřených tabů) na bool.
        /// </summary>
        /// <param name="value">Hodnota vstupu (true = zvětšený, false = normální velikost).</param>
        /// <param name="targetType">Cílový typ převodu (nepoužívá se).</param>
        /// <param name="parameter">Parametr určující, zda jde o výšku, šířku nebo počet sloupců.</param>
        /// <param name="culture">Kultura (nepoužívá se).</param>
        /// <returns>Vrací dynamicky vypočítanou velikost prvku na základě stavu rozbalení.</returns>
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int count)
            {
                bool isVisible = count > 0;

                // Pokud máme parameter "invert", vrátíme opačnou hodnotu
                if (parameter is string param && param == "invert")
                {
                    return !isVisible;
                }

                return isVisible;
            }
            return false;
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
            throw new NotImplementedException();
        }
    }
}
