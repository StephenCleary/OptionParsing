using System;
using Nito.OptionParsing.Converters;

namespace Nito.OptionParsing
{
    /// <summary>
    /// Specifies that a command-line option sets this property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public sealed class OptionAttribute: Attribute
    {
        /// <param name="longName">The long name of the option. The long name may not contain ':' or '=' characters.</param>
        /// <param name="argument">Whether this option takes an argument.</param>
        public OptionAttribute(string longName, OptionArgument argument = OptionArgument.Required)
        {
            LongName = longName;
            Argument = argument;
        }

        /// <param name="shortName">The short name of the option. The short name may not be ':' or '='.</param>
        /// <param name="argument">Whether this option takes an argument.</param>
        public OptionAttribute(char shortName, OptionArgument argument = OptionArgument.Required)
        {
            ShortName = shortName;
            Argument = argument;
        }

        /// <param name="longName">The long name of the option. The long name may not contain ':' or '=' characters.</param>
        /// <param name="shortName">The short name of the option. The short name may not be ':' or '='.</param>
        /// <param name="argument">Whether this option takes an argument.</param>
        public OptionAttribute(string longName, char shortName, OptionArgument argument = OptionArgument.Required)
        {
            LongName = longName;
            ShortName = shortName;
            Argument = argument;
        }

        /// <summary>
        /// The long name of the option, if any. The long name may not contain ':' or '=' characters.
        /// </summary>
        public string LongName { get; set; }

        /// <summary>
        /// The short name of the option, if any. The short name may not be ':' or '='.
        /// </summary>
        public char? ShortName { get; set; }

        /// <summary>
        /// Whether this option takes an argument.
        /// </summary>
        public OptionArgument Argument { get; set; }

        /// <summary>
        /// Specifies a custom converter for this property. The converter must implement <see cref="IOptionArgumentValueConverter"/>.
        /// </summary>
        public Type Converter { get; set; }
    }
}
