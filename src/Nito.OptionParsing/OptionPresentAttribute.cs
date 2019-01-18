using System;

namespace Nito.OptionParsing
{
    /// <summary>
    /// Specifies that the presence of a command-line option sets this property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class OptionPresentAttribute: Attribute
    {
        /// <param name="longName">The long name of the option. The long name may not contain ':' or '=' characters.</param>
        public OptionPresentAttribute(string longName)
        {
            LongName = longName;
        }

        /// <param name="shortName">The short name of the option, if any. The short name may not be ':' or '='.</param>
        public OptionPresentAttribute(char shortName)
        {
            ShortName = shortName;
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
        /// Gets the short name of the option as a string.
        /// </summary>
        public string ShortNameAsString => ShortName?.ToString();

        /// <summary>
        /// Gets the long name, if any; otherwise, gets the short name, if any.
        /// </summary>
        public string Name => LongName ?? ShortNameAsString;
    }
}
