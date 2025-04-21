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
