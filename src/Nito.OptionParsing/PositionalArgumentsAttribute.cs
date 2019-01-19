using System;
using Nito.OptionParsing.Converters;

namespace Nito.OptionParsing
{
    /// <summary>
    /// Specifies that the command-line sets this property to the remaining positional arguments. The property type must be a collection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class PositionalArgumentsAttribute: Attribute
    {
        /// <summary>
        /// Specifies a custom converter for this property. The converter must implement <see cref="IOptionArgumentValueConverter"/>.
        /// </summary>
        public Type Converter { get; set; }
    }
}
