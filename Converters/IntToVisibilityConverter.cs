using System.Globalization;

namespace project.Converters
{
    /// <summary>
    /// Converts an integer value (e.g., number of open tabs) into a boolean visibility flag.
    /// Used to control visibility of UI elements based on tab presence.
    /// </summary>
    public class IntToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Forces the compiler to include this converter.
        /// </summary>
        public static void ForceCompile() { }

        /// <summary>
        /// Converts an integer to a boolean. If the count is greater than 0, returns true; otherwise false.
        /// If the parameter is "invert", the result is negated.
        /// </summary>
        /// <param name="value">The input value (expected to be an integer).</param>
        /// <param name="targetType">The target binding type (unused).</param>
        /// <param name="parameter">Optional parameter to invert the result.</param>
        /// <param name="culture">The culture information (unused).</param>
        /// <returns>True if count > 0 (or false if inverted); otherwise false.</returns>
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int count)
            {
                bool isVisible = count > 0;

                if (parameter is string param && param == "invert")
                {
                    return !isVisible;
                }

                return isVisible;
            }
            return false;
        }

        /// <summary>
        /// ConvertBack is not supported for this converter.
        /// </summary>
        /// <param name="value">The input value.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">Optional parameter.</param>
        /// <param name="culture">The culture info.</param>
        /// <returns>Throws NotImplementedException.</returns>
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
