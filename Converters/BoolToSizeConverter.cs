using System;
using System.Globalization;
using Microsoft.Maui.Devices; // Pro získání informací o obrazovce zařízení
using Microsoft.Maui.Controls;

namespace project.Converters;

/// <summary>
/// Převádí hodnotu bool (zda je graf zvětšený) na odpovídající velikost.
/// Používá se k dynamickému nastavování velikosti prvků v závislosti na jejich stavu.
/// </summary>
public class BoolToSizeConverter : IValueConverter
{
    /// <summary>
    /// Převádí bool hodnotu (stav rozbalení) na velikost prvku.
    /// </summary>
    /// <param name="value">Hodnota vstupu (true = zvětšený, false = normální velikost).</param>
    /// <param name="targetType">Cílový typ převodu (nepoužívá se).</param>
    /// <param name="parameter">Parametr určující, zda jde o výšku, šířku nebo počet sloupců.</param>
    /// <param name="culture">Kultura (nepoužívá se).</param>
    /// <returns>Vrací dynamicky vypočítanou velikost prvku na základě stavu rozbalení.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isExpanded && parameter is string param) // Ověření vstupních hodnot
        {
            // Získání velikosti obrazovky zařízení v logických jednotkách
            double screenWidth = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;
            double screenHeight = DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density;

            // Dostupná šířka pro pravý sloupec (např. 75 % obrazovky)
            double availableWidth = screenWidth * 0.75;

            // Pokud se parametr týká výšky, nastavíme výšku podle stavu rozbalení
            if (param == "height")
                return isExpanded ? screenHeight * 0.8 : screenHeight * 0.4; // Rozbalený = 80 % výšky, jinak 40 %

            // Pokud se parametr týká šířky, zvětšíme ji nebo rozdělíme na dvě části
            if (param == "width")
                return isExpanded ? availableWidth : availableWidth / 2 - 10; // Dva sloupce ve výchozím stavu

            // Pokud se parametr týká rozložení sloupců, rozhodujeme mezi 1 a 2 sloupci
            if (param == "span")
                return isExpanded ? 1 : 2; // Rozbalený graf zabere 1 sloupec, jinak jsou 2 sloupce vedle sebe
        }

        // Výchozí hodnota, pokud vstupní podmínky neodpovídají
        return 300;
    }

    /// <summary>
    /// Nepodporovaná zpětná konverze (není potřeba).
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
