using System.Globalization;

namespace project.Converters
{
    /// <summary>
    /// Universal converter that returns various layout-related values based on a boolean input.
    /// Used primarily to adjust size and visibility based on expansion state.
    /// </summary>
    public class BoolToSizeConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean value into a size, span count, layout configuration, or visibility.
        /// </summary>
        /// <param name="value">Boolean value, usually bound to IsExpanded.</param>
        /// <param name="targetType">Target binding type (not used).</param>
        /// <param name="parameter">Determines the type of value to return: "width", "height", "span", "spanLayout", "invert", or "visibility".</param>
        /// <param name="culture">Culture information (not used).</param>
        /// <returns>The corresponding converted value based on the parameter.</returns>
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isExpanded && parameter is string param)
            {
                double screenWidth = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;
                double screenHeight = DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density;

                double rightAreaWidth = screenWidth - 300 - 40;
                double usableHeight = screenHeight - 100;

                return param switch
                {
                    "width" => isExpanded ? rightAreaWidth : (rightAreaWidth / 2) - 20,
                    "height" => isExpanded ? usableHeight : usableHeight / 2,
                    "span" => isExpanded ? 1 : 2,
                    "spanLayout" => isExpanded
                        ? new GridItemsLayout(1, ItemsLayoutOrientation.Vertical)
                        : new GridItemsLayout(2, ItemsLayoutOrientation.Vertical),
                    "invert" => !isExpanded,
                    "visibility" => !isExpanded,
                    _ => 300
                };
            }

            return 300;
        }

        /// <summary>
        /// ConvertBack is not supported for this converter.
        /// </summary>
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
