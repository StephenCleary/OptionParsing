using System;
using System.Collections.Generic;
using System.Text;

namespace Nito.OptionParsing.Converters
{
    /// <summary>
    /// A custom converter for an option argument value.
    /// </summary>
    public interface IOptionArgumentValueConverter
    {
        /// <summary>
        /// Whether this converter can convert argument values of the specified type.
        /// </summary>
        /// <param name="type">The target type.</param>
        bool CanConvert(Type type);

        /// <summary>
        /// Attempts to convert a string into a target type. Returns <c>null</c> if it cannot make this conversion.
        /// </summary>
        /// <param name="type">The target type.</param>
        /// <param name="text">The string to convert.</param>
        object TryConvert(Type type, string text);
    }
}
