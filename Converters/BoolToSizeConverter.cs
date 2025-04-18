using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;

namespace project.Converters
{
    /// <summary>
    /// Univerzální konvertor, který na základě boolean hodnoty (např. IsExpanded)
    /// vrací různé typy hodnot podle zadaného parametru (šířka, výška, počet sloupců...).
    /// </summary>
    public class BoolToSizeConverter : IValueConverter
    {
        /// <summary>
        /// Hlavní konverzní logika.
        /// Podle parametru vrací buď výšku, šířku, počet sloupců, layout nebo viditelnost.
        /// </summary>
        /// <param name="value">Hodnota typu bool – obvykle IsExpanded u grafu.</param>
        /// <param name="targetType">Cílový typ bindingu.</param>
        /// <param name="parameter">Řetězec určující typ výstupu (např. „width“, „height“, „span“...).</param>
        /// <param name="culture">Kulturní informace (nevyužívá se).</param>
        /// <returns>Výstup odpovídající danému parametru.</returns>
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isExpanded && parameter is string param)
            {
                // Aktuální rozměry zařízení v jednotkách logických bodů (DIP)
                double screenWidth = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;
                double screenHeight = DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density;

                // Prostor pro hlavní část s grafy (po odečtení sidebaru a paddingu)
                double rightAreaWidth = screenWidth - 300 - 40;

                // Výška dostupné oblasti pro grafy (bez horní lišty)
                double usableHeight = screenHeight - 100;

                // Vrací šířku grafu (celá vs. poloviční)
                if (param == "width")
                {
                    return isExpanded ? rightAreaWidth : (rightAreaWidth / 2) - 20;
                }

                // Vrací výšku grafu
                if (param == "height")
                {
                    return isExpanded ? usableHeight : usableHeight / 2;
                }

                // Vrací počet sloupců v layoutu (např. 1 při rozbalení)
                if (param == "span")
                {
                    return isExpanded ? 1 : 2;
                }

                // Vrací samotný layout pro CollectionView (GridItemsLayout)
                if (param == "spanLayout")
                {
                    return isExpanded
                        ? new GridItemsLayout(1, ItemsLayoutOrientation.Vertical)
                        : new GridItemsLayout(2, ItemsLayoutOrientation.Vertical);
                }

                // Logická inverze – např. pro zobrazení tlačítka "+"
                if (param == "invert")
                {
                    return !isExpanded;
                }

                // Používá se např. pro nastavení Visibility (true → false)
                if (param == "visibility")
                {
                    return !isExpanded;
                }
            }

            // Výchozí návratová hodnota, pokud nebyly splněny podmínky
            return 300;
        }

        /// <summary>
        /// Zpětná konverze není podporována.
        /// </summary>
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
