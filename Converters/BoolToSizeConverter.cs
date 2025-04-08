using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;

namespace project.Converters
{
    public class BoolToSizeConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isExpanded && parameter is string param)
            {
                double screenWidth = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;
                double screenHeight = DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density;

                // Prostor vpravo (hlavní oblast)
                double rightAreaWidth = screenWidth - 300 - 40; // 300 levý panel, 40 mezery

                // Výška hlavní oblasti (spodní část okna)
                double usableHeight = screenHeight - 100; // Odečtení horní lišty

                if (param == "width")
                {
                    return isExpanded ? rightAreaWidth : (rightAreaWidth / 2) - 20;
                }

                if (param == "height")
                {
                    return isExpanded ? usableHeight : usableHeight / 2;
                }

                if (param == "span")
                {
                    return isExpanded ? 1 : 2;
                }

                if (param == "spanLayout")
                {
                    return isExpanded
                        ? new GridItemsLayout(1, ItemsLayoutOrientation.Vertical)
                        : new GridItemsLayout(2, ItemsLayoutOrientation.Vertical);
                }

                if (param == "invert")
                {
                    return !isExpanded;
                }

                if (param == "visibility")
                {
                    return !isExpanded;
                }
            }

            return 300;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
