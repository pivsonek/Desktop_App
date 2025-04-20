using System.Collections;
using System.Globalization;

namespace project.Converters
{
    /// <summary>
    /// Converts a collection to a boolean value indicating whether the collection is non-empty.
    /// </summary>
    public class CollectionToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts a collection to a boolean. Returns true if the collection is not empty, otherwise false.
        /// </summary>
        /// <param name="value">The input value (expected to be ICollection).</param>
        /// <param name="targetType">The target binding type (unused).</param>
        /// <param name="parameter">Optional parameter (unused).</param>
        /// <param name="culture">The culture information (unused).</param>
        /// <returns>True if the collection has items, otherwise false.</returns>
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is ICollection collection && collection.Count > 0;
        }

        /// <summary>
        /// ConvertBack is not supported for this converter.
        /// </summary>
        /// <param name="value">The input value.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">Optional parameter.</param>
        /// <param name="culture">The culture info.</param>
        /// <returns>Throws NotImplementedException.</returns>
        /// <exception cref="NotImplementedException">Always thrown.</exception>
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
