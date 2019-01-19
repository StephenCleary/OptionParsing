using System;
using Nito.OptionParsing.Converters;

namespace Nito.OptionParsing
{
    /// <summary>
    /// Specifies that a command-line positional argument sets this property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class PositionalArgumentAttribute: Attribute
    {
        /// <param name="index">The index of the positional argument.</param>
        public PositionalArgumentAttribute(int index)
        {
            Index = index;
        }

        /// <summary>
        /// The index of the positional argument.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Specifies a custom converter for this property. The converter must implement <see cref="IOptionArgumentValueConverter"/>.
        /// </summary>
        public Type Converter { get; set; }
    }
}
