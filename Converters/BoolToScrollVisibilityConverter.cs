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
